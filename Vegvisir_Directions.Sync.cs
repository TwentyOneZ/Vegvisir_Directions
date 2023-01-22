using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using UnityEngine;

[BepInPlugin("twentyonez.valheim.mods.vegvisirDirections", "vegvisirDirections", "1.0.0")]
[BepInIncompatibility("com.bepinex.plugins.vegvisirDirections")]
public class vegvisirDirections : BaseUnityPlugin
{
	[HarmonyPatch(typeof(Game), "RPC_DiscoverLocationRespons")]
	private class vegvisirDirections_Patch
	{
		private static bool Prefix(long sender, string pinName, int pinType, Vector3 pos)
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
			if (vegvisirDirections.showPreciseDirection.Value)
			{
				switch (Mathf.CeilToInt((float)num2 / 45f))
				{
					case 0:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.N_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 1:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.NE_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 2:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.E_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 3:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.SE_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 4:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.S_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 5:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.SW_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 6:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.W_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 7:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.NW_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
					case 8:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.N_string.Value + "\n(" + num2.ToString() + "° clockwise from North)", autoHide: true);
						break;
				}
			}
			else
			{
				switch (Mathf.CeilToInt((float)num2 / 45f))
				{
					case 0:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.N_string.Value, autoHide: true);
						break;
					case 1:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.NE_string.Value, autoHide: true);
						break;
					case 2:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.E_string.Value, autoHide: true);
						break;
					case 3:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.SE_string.Value, autoHide: true);
						break;
					case 4:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.S_string.Value, autoHide: true);
						break;
					case 5:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.SW_string.Value, autoHide: true);
						break;
					case 6:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.W_string.Value, autoHide: true);
						break;
					case 7:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.NW_string.Value, autoHide: true);
						break;
					case 8:
						TextViewer.instance.ShowText(TextViewer.Style.Rune, pinName + " Vegvisir", vegvisirDirections.N_string.Value, autoHide: true);
						break;
				}
			}
			if (vegvisirDirections.pinLocationOnMap.Value)
			{
				Minimap.instance.DiscoverLocation(pos, (Minimap.PinType)pinType, pinName, false);
			}
			if (vegvisirDirections.stareAtMarkedDirection.Value)
			{
				Player.m_localPlayer.SetLookDir(pos - Player.m_localPlayer.transform.position, (float)vegvisirDirections.turningDelay.Value);
			}
			else if (vegvisirDirections.stareAtNorth.Value)
			{
				Player.m_localPlayer.SetLookDir(Vector3.forward.normalized, (float)vegvisirDirections.turningDelay.Value);
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

	public static ConfigEntry<bool> showPreciseDirection;
	public static ConfigEntry<bool> pinLocationOnMap;
	public static ConfigEntry<bool> stareAtMarkedDirection;
	public static ConfigEntry<bool> stareAtNorth;
	public static ConfigEntry<float> turningDelay;
	public static ConfigEntry<string> N_string;
	public static ConfigEntry<string> NE_string;
	public static ConfigEntry<string> E_string;
	public static ConfigEntry<string> SE_string;
	public static ConfigEntry<string> S_string;
	public static ConfigEntry<string> SW_string;
	public static ConfigEntry<string> W_string;
	public static ConfigEntry<string> NW_string;


	public readonly Harmony harmony = new Harmony("twentyoneZ.Vegvisir_Directions");

	public const string Version = "1.0.0";

	private const string Name = "Vegvisir_Directions";

	public const string Guid = "twentyoneZ.valheim.mods.Vegvisir_Directions";

	private static readonly Vector3[] _sidesDirections = new Vector3[360];

	private void Awake()
	{
		Config.SaveOnConfigSet = true;

		showPreciseDirection = Config.Bind<bool>("General", "showPreciseDirection", false,
			new ConfigDescription("Reveals the precise angle in the compass", null));
		pinLocationOnMap = Config.Bind<bool>("General", "pinLocationOnMap", true,
			new ConfigDescription("Marks the location on the map anyways", null));
		stareAtMarkedDirection = Config.Bind<bool>("General", "stareAtMarkedDirection", true,
			new ConfigDescription("Change the player camera direction to the marked direction", null));
		stareAtNorth = Config.Bind<bool>("General", "stareAtNorth", false,
			new ConfigDescription("Change the player camera direction to the North (disabled if stareAtMarkedDirection is true)", null));
		turningDelay = Config.Bind<float>("General", "turningDelay", 1.5f,
			new ConfigDescription("Time in seconds it takes to turn the camera angle to the marked direction", null));
		N_string = Config.Bind<string>("Localization", "N_string", "You feel a tingling sensation pointing North.",
			new ConfigDescription("String to describe North direction", null));
		NE_string = Config.Bind<string>("Localization", "NE_string", "A threatening breeze comes from Northeast.",
			new ConfigDescription("String to describe Northeast direction", null));
		E_string = Config.Bind<string>("Localization", "E_string", "You sense a disturbance coming from East.",
			new ConfigDescription("String to describe East direction", null));
		SE_string = Config.Bind<string>("Localization", "SE_string", "Your instincts tell there is a menace to the Southeast.",
			new ConfigDescription("String to describe Southeast direction", null));
		S_string = Config.Bind<string>("Localization", "S_string", "South seems to be where you are looking for.",
			new ConfigDescription("String to describe South direction", null));
		SW_string = Config.Bind<string>("Localization", "SW_string", "A dark wind blows from Southwest.",
			new ConfigDescription("String to describe Southwest direction", null));
		W_string = Config.Bind<string>("Localization", "W_string", "Staring at the vegvisir makes you visualize an arrow pointing to West.",
			new ConfigDescription("String to describe West direction", null));
		NW_string = Config.Bind<string>("Localization", "NW_string", "You hear a deafening sound coming from Northwest. But in an instant it is gone, making you wonder if it was just your imagination.",
			new ConfigDescription("String to describe Northwest direction", null));


		harmony.PatchAll();
	}
}