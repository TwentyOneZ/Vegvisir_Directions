using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
//using Wayshrine;

namespace VegvisirDirections;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("TargetPortal", BepInDependency.DependencyFlags.SoftDependency)]
//[BepInDependency("AzuWayshrine", BepInDependency.DependencyFlags.SoftDependency)]
public class VegvisirDirections : BaseUnityPlugin
{
	public const string ModName = "Vegvisir Directions";
	public const string ModVersion = "1.0.4";
	public const string ModGUID = "twentyOneZ.VegvisirDirections";

	public static readonly ConfigSync configSync = new(ModName) { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };
	public static ConfigEntry<Toggle> serverConfigLocked;
	public static ConfigEntry<Toggle> showPreciseDirection;
	public static ConfigEntry<Toggle> pinLocationOnMap;
	public static ConfigEntry<Toggle> stareAtMarkedDirection;
	public static ConfigEntry<Toggle> stareAtNorth;
	public static ConfigEntry<float> turningDelay;
	public static ConfigEntry<Toggle> syncLocalization;
	public static ConfigEntry<Toggle> restrictMapToCartographyTable;
	//public static ConfigEntry<Toggle> wayshrinesCompatibility;
	public static ConfigEntry<Toggle> targetPortalCompatibility;
	public static ConfigEntry<string> mapAccessWhenStatusEffectIsUp;
	public static ConfigEntry<string> mapAcessWhenStatusEffectIsUpAfterCartographyTable;
	public static ConfigEntry<string> firstAcessToCartographyTable_String;
	public static ConfigEntry<string> N_string;
	public static ConfigEntry<string> NE_string;
	public static ConfigEntry<string> E_string;
	public static ConfigEntry<string> SE_string;
	public static ConfigEntry<string> S_string;
	public static ConfigEntry<string> SW_string;
	public static ConfigEntry<string> W_string;
	public static ConfigEntry<string> NW_string;

	private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
	{
		ConfigEntry<T> configEntry = Config.Bind(group, name, value, description);

		SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
		syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

		return configEntry;
	}

	private ConfigEntry<T> config<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => config(group, name, value, new ConfigDescription(description), synchronizedSetting);

	public enum Toggle
	{
		On = 1,
		Off = 0
	}

	public static readonly Vector3[] _sidesDirections = new Vector3[360];

	public void Awake()
	{
		Config.SaveOnConfigSet = true;

		serverConfigLocked = config("1 - Server Sync Configurations", "Lock Configuration", Toggle.On, "If on, the configuration is locked and can be changed by server admins only.");
		configSync.AddLockingConfigEntry(serverConfigLocked);

		syncLocalization = config<Toggle>("1 - Server Sync Configurations", "Sychronize Localization", Toggle.On,
			new ConfigDescription("Should Localization config be synchronized with the server (on) or leave it to clients (off)"), true);

		showPreciseDirection = config<Toggle>("1 - Server Sync Configurations", "Show precise direction", Toggle.Off,
			"Reveals the precise angle in the compass", true);

		pinLocationOnMap = config<Toggle>("1 - Server Sync Configurations", "Mark altar location", Toggle.On,
			"Marks the altar location on the map anyways", true);

		stareAtMarkedDirection = config<Toggle>("1 - Server Sync Configurations", "Stare at marked direction", Toggle.On,
			"Change the player camera direction to the marked direction", true);

		stareAtNorth = config<Toggle>("1 - Server Sync Configurations", "Stare at north", Toggle.Off,
			"Change the player camera direction to the North (disabled if 'Stare at marked direction' is On)", true);

		restrictMapToCartographyTable = config<Toggle>("1 - Server Sync Configurations", "Map Restriction", Toggle.Off,
			"Restricts opening the world map to interacting with the cartography table. Also disables the minimap.", true);

		mapAccessWhenStatusEffectIsUp = config<string>("1 - Server Sync Configurations", "Map enabled with this effect", "Wishbone",
			new ConfigDescription("Makes map acessible when the character have this status effect. Leave it blank to disable. Disabled if Map Restriction is Off. \nDefault value makes map acessible when a wishbone is equipped.\nPossible values are the names of the status effects, such as Resting, Rested, Wishbone, Fire, etc."), true);

		mapAcessWhenStatusEffectIsUpAfterCartographyTable = config<string>("1 - Server Sync Configurations", "Map enabled with this effect after visiting a cartography table", "Resting",
			new ConfigDescription("Makes map acessible when the character have this status effect, but only after acessing a cartography table for the first time. Leave it blank to disable. Disabled if Map Restriction is Off. \nDefault value makes map acessible whenever the character is Rested, but only after using a cartography table for the first time.\nPossible values are the names of the status effects, such as Resting, Rested, Wishbone, Fire, etc."), true);

		turningDelay = config<float>("1 - Server Sync Configurations", "Turning Delay", 1.5f,
			new ConfigDescription("Time in seconds it takes to turn the camera angle to the marked direction", new AcceptableValueRange<float>(1f, 3f)), true);

		//wayshrinesCompatibility = config<Toggle>("1 - Server Sync Configurations", "Wayshrines mod Compatibility", Toggle.Off,
		//	"Makes mod compatible with Wayshrines mod. Opens the map whenever you get near a wayshrine.", true);

		targetPortalCompatibility = config<Toggle>("1 - Server Sync Configurations", "TargetPortal mod Compatibility", Toggle.Off,
			"Makes mod compatible with Wayshrines mod. Opens the map whenever you enter a portal.", true);

		firstAcessToCartographyTable_String = config<string>("2 - Localization", "firstAcessToCartographyTable_String", "You take one of maps on the table with you.",
			new ConfigDescription("String to describe North direction"), VegvisirDirections.syncLocalization.Value == Toggle.On);

		N_string = config<string>("2 - Localization", "N_string", "As you touch the vegvisir, you can't shake off the feeling that danger lurks just beyond the north.",
			new ConfigDescription("String to describe North direction"), VegvisirDirections.syncLocalization.Value == Toggle.On);

		NE_string = config<string>("2 - Localization", "NE_string", "Touching the vegvisir, you sense that something sinister lies to the northeast, waiting to be awaken.",
			new ConfigDescription("String to describe Northeast direction"), VegvisirDirections.syncLocalization.Value == Toggle.On);

		E_string = config<string>("2 - Localization", "E_string", "You feel a sense of unease when looking towards the east through the vegvisir, as if something terrible lies in that direction.",
			new ConfigDescription("String to describe East direction"), VegvisirDirections.syncLocalization.Value == Toggle.On);

		SE_string = config<string>("2 - Localization", "SE_string", "The vegvisir reveals to you that the southeast holds ominous possibilities, and you can't shake off the feeling of dread.",
			new ConfigDescription("String to describe Southeast direction"), VegvisirDirections.syncLocalization.Value == Toggle.On);

		S_string = config<string>("2 - Localization", "S_string", "After touching the vegvisir, you feel a sense of impending doom when you look towards the south, as if something dreadful is dormant there.",
			new ConfigDescription("String to describe South direction"), VegvisirDirections.syncLocalization.Value == Toggle.On);

		SW_string = config<string>("2 - Localization", "SW_string", "You feel the vegvisir guiding you towards the southwest, filling you with a sense of unease, as if something terrible is waiting for you there.",
			new ConfigDescription("String to describe Southwest direction"), VegvisirDirections.syncLocalization.Value == Toggle.On);

		W_string = config<string>("2 - Localization", "W_string", "The power of the vegvisir alerts you the west holds a sense of danger that you can't quite put your finger on.",
			new ConfigDescription("String to describe West direction"), VegvisirDirections.syncLocalization.Value == Toggle.On);

		NW_string = config<string>("2 - Localization", "NW_string", "As you put your hand on the vegvisir, you sense the northwest feels ominous and foreboding, as if something terrible is lurking just beyond.",
			new ConfigDescription("String to describe Northwest direction"), VegvisirDirections.syncLocalization.Value == Toggle.On);

		Assembly assembly = Assembly.GetExecutingAssembly();
		Harmony harmony = new(ModGUID);
		harmony.PatchAll(assembly);
	}

	[HarmonyPatch(typeof(Game), "RPC_DiscoverLocationRespons")]
	public class VegvisirDirections_Patch
	{
		public static bool Prefix(long sender, string pinName, int pinType, Vector3 pos)
		{
			Vector3 position = Player.m_localPlayer.transform.position;

			Vector3 vector = pos - position;
			vector.y = 0f;
			vector = vector.normalized;
			float num = 0f;
			int num2 = 0;
			Get_Sides_Direction(_sidesDirections);
			for (int i = 0; i < 360; i++)
			{
				float num3 = Vector3.Dot(_sidesDirections[i], vector);
				if (num3 > num)
				{
					num = num3;
					num2 = i;
				}
			}
			if (VegvisirDirections.showPreciseDirection.Value == Toggle.On)
			{
				switch (Mathf.RoundToInt((float)num2 / 45f))
				{
					case 0:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.N_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 1:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.NE_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 2:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.E_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 3:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.SE_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 4:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.S_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 5:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.SW_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 6:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.W_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 7:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.NW_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 8:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.N_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
				}
			}
			else
			{
				switch (Mathf.RoundToInt((float)num2 / 45f))
				{
					case 0:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.N_string.Value, autoHide: true);
						break;
					case 1:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.NE_string.Value, autoHide: true);
						break;
					case 2:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.E_string.Value, autoHide: true);
						break;
					case 3:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.SE_string.Value, autoHide: true);
						break;
					case 4:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.S_string.Value, autoHide: true);
						break;
					case 5:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.SW_string.Value, autoHide: true);
						break;
					case 6:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.W_string.Value, autoHide: true);
						break;
					case 7:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.NW_string.Value, autoHide: true);
						break;
					case 8:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", VegvisirDirections.N_string.Value, autoHide: true);
						break;
				}
			}
			if (VegvisirDirections.pinLocationOnMap.Value == Toggle.On)
			{
				Minimap.instance.DiscoverLocation(pos, (Minimap.PinType)pinType, pinName, false);
			}
			if (VegvisirDirections.stareAtMarkedDirection.Value == Toggle.On)
			{
				Player.m_localPlayer.SetLookDir(pos - Player.m_localPlayer.transform.position, (float)VegvisirDirections.turningDelay.Value);
			}
			else if (VegvisirDirections.stareAtNorth.Value == Toggle.On)
			{
				Player.m_localPlayer.SetLookDir(Vector3.forward.normalized, (float)VegvisirDirections.turningDelay.Value);
			}
			return false;

		}

		private static Vector3[] Get_Sides_Direction(Vector3[] __sidesDirection)
		{
			for (int i = 0; i < 360; i++)
			{
				_sidesDirections[i] = Quaternion.Euler(Vector3.up * 1.0f * i) * Vector3.forward.normalized;
			}
			return __sidesDirection;
		}
	}

	[HarmonyPatch(typeof(Minimap), "Update")]
	static class Minimap_Update_Patch
	{
		static void Postfix(Minimap __instance, Minimap.MapMode ___m_mode)
		{
			if (Player.m_localPlayer == null || !(VegvisirDirections.restrictMapToCartographyTable.Value == Toggle.On) || IsPlayerMapAllowed())
				return;
			__instance.m_smallRoot.SetActive(false);
			if (ZInput.GetButtonDown("Map") || ZInput.GetButtonDown("JoyMap") || ZInput.GetButtonDown("JoyMap"))
			{
				__instance.SetMapMode(Minimap.MapMode.None);
			} 
		}
	}
	[HarmonyPatch(typeof(MapTable), "OnRead")]
	static class MapTable_OnRead_Patch
	{
		static void Postfix(MapTable __instance, ItemDrop.ItemData item)
		{
			if (Player.m_localPlayer == null || item != null || !(VegvisirDirections.restrictMapToCartographyTable.Value == Toggle.On) || IsPlayerMapAllowed())
				return;
			Minimap.instance.SetMapMode(Minimap.MapMode.Large);
			if (!VegvisirDirections.mapAcessWhenStatusEffectIsUpAfterCartographyTable.Value.Length.Equals(0) && !Player.m_localPlayer.HaveUniqueKey("mapAcquired"))
			{
				Player.m_localPlayer.AddUniqueKey("mapAcquired");
				Player.m_localPlayer.Message(MessageHud.MessageType.Center, VegvisirDirections.firstAcessToCartographyTable_String.Value);
			}
			return;
		}
	}
	[HarmonyPatch(typeof(Minimap), nameof(Minimap.ShowPointOnMap))]
	static class Minimap_ShowPointOnMap_Patch
	{
		static bool Prefix()
		{
			if (Player.m_localPlayer != null && VegvisirDirections.restrictMapToCartographyTable.Value == Toggle.On || !IsPlayerMapAllowed()) {
				return false;
			}
			return true;
		}
	}

	//[HarmonyPatch(typeof(WayshrineCustomBehaviour), nameof(WayshrineCustomBehaviour.Interact))]
	//static class WayshrineInteractPatch
	//{
	//	static bool Prefix()
	//	{
	//		if (isAwayFromWayshrine)
    //       {
	//			Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Come closer to the wayshrine");
	//			return false;
    //        } else
    //        {
	//			isAwayFromWayshrine = false;
	//			Minimap.instance.SetMapMode(Minimap.MapMode.Large);
	//			return true;
	//		}
	//	}
	//}

	public static bool isAwayFromPortal = true;
	//public static bool isAwayFromWayshrine = true;
	public static bool IsPlayerMapAllowed()
	{
	//	if ((VegvisirDirections.wayshrinesCompatibility.Value == Toggle.On)) 
	//	{ 
	//		List<Piece> m_tempPieces = new List<Piece>();
	//		m_tempPieces.Clear();
	//		Piece.GetAllPiecesInRadius(Player.m_localPlayer.transform.position, 2.0f, m_tempPieces);
	//		if (m_tempPieces != null)
	//		{
	//			foreach (Piece onePiece in m_tempPieces)
	//			{
	//				if (onePiece.gameObject != null && (onePiece.gameObject.name.ToLower().Contains("wayshrine")))
	//				{
	//					if (isAwayFromWayshrine)
	//					{
	//						GameObject hoverObject = onePiece.gameObject;
	//						if (!(hoverObject == null))
	//						{
	//							Interactable componentInParent = hoverObject.GetComponentInParent<Interactable>();
	//							Debug.Log($"Component: {componentInParent}, HoverName:{hoverObject.name}");
	//							if (componentInParent is Wayshrine.WayshrineCustomBehaviour)
	//							{
	//								isAwayFromWayshrine = false;
	//								componentInParent.Interact(Player.m_localPlayer, false, false);
	//								return true;
	//							}
	//						}
	//					}
	//					return true;
	//				}
	//			}
	//		}
	//		if (!isAwayFromWayshrine)
	//		{
	//			Minimap.instance.SetMapMode(Minimap.MapMode.None);
	//			isAwayFromWayshrine = true;
	//		}
	//	}
		if ((VegvisirDirections.targetPortalCompatibility.Value == Toggle.On))
		{
			List<Piece> m_tempPieces = new List<Piece>();
			m_tempPieces.Clear();
			Piece.GetAllPiecesInRadius(Player.m_localPlayer.transform.position, 1f, m_tempPieces);
			if (m_tempPieces != null)
			{
				foreach (Piece onePiece in m_tempPieces)
				{
					if (onePiece.gameObject != null && (onePiece.gameObject.name.ToLower().Contains("portal")))
					{
						if (isAwayFromPortal)
						{
							isAwayFromPortal = false;
							Minimap.instance.SetMapMode(Minimap.MapMode.Large);
						}
						return true;
					} 
				}
			}
			if (!isAwayFromPortal)
			{
				Minimap.instance.SetMapMode(Minimap.MapMode.None);
				isAwayFromPortal = true;
			}
		}
		if (!VegvisirDirections.mapAccessWhenStatusEffectIsUp.Value.Length.Equals(0) && Player.m_localPlayer.GetSEMan().HaveStatusEffect(VegvisirDirections.mapAccessWhenStatusEffectIsUp.Value))
		{
			return true;
		}
		if (!VegvisirDirections.mapAcessWhenStatusEffectIsUpAfterCartographyTable.Value.Length.Equals(0) && Player.m_localPlayer.GetSEMan().HaveStatusEffect(VegvisirDirections.mapAcessWhenStatusEffectIsUpAfterCartographyTable.Value) && Player.m_localPlayer.HaveUniqueKey("mapAcquired"))
		{
			return true;
		}
		return false;
	}

}