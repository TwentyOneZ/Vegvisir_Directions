using System.Reflection;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using EpicMMOSystem;
using ValheimLegends;
using System.Collections.Generic;
using System.Linq;

namespace VL_DamageFix
{
	[BepInPlugin(ModGUID, ModName, ModVersion)]
	[BepInDependency("ValheimLegends", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("EpicMMOSystem", BepInDependency.DependencyFlags.SoftDependency)]

	public class VL_DamageFix : BaseUnityPlugin
	{
		private const string ModName = "VL_DamageFix";
		private const string ModVersion = "0.0.1";
		private const string ModGUID = "twentyone.VL_DamageFix";

		public void Awake()
		{

			Assembly assembly = Assembly.GetExecutingAssembly();
			Harmony harmony = new(ModGUID);
			harmony.PatchAll(assembly);

		}

		[HarmonyPatch(typeof(Attack), "DoMeleeAttack", null)]
		public class MeleeAttack_Patch
		{
			public static bool Prefix(Attack __instance, Humanoid ___m_character, ref float ___m_damageMultiplier)
			{
				if (___m_character.GetSEMan().HaveStatusEffect("SE_VL_Berserk"))
				{
					ValheimLegends.SE_Berserk sE_Berserk = (ValheimLegends.SE_Berserk)___m_character.GetSEMan().GetStatusEffect("SE_VL_Berserk");
					___m_damageMultiplier = Mathf.Clamp(___m_damageMultiplier * sE_Berserk.damageModifier, 1f, 1.2f);
					HitData hitData = new HitData();
					if (___m_character.GetHealth() > __instance.m_attackStamina)
					{
						hitData.m_damage.m_spirit = Mathf.Clamp(__instance.m_attackStamina, 1f, 0.10f * ___m_character.GetHealth());
						hitData.m_point = ___m_character.GetEyePoint();
						___m_character.ApplyDamage(hitData, showDamageText: true, triggerEffects: true);
						Object.Instantiate(ZNetScene.instance.GetPrefab("fx_deathsquito_hit"), ___m_character.GetCenterPoint(), Quaternion.identity);
					} else
					{
						___m_character.GetSEMan().RemoveStatusEffect("SE_VL_Berserk");
					}
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(Humanoid), "GetCurrentWeapon")]
		public static class UnarmedDamage
		{
			private static ItemDrop.ItemData Postfix(ItemDrop.ItemData __weapon, ref Character __instance)
			{
				if (__weapon != null && __weapon.m_shared.m_name == "Unarmed")
				{
					Player player = (Player)__instance;
					__weapon.m_shared.m_damages.m_blunt = (2 + (player.GetSkillFactor(Skills.SkillType.Unarmed)) / (100 / 3)) * (1f + (EpicMMOSystem.LevelSystem.Instance.getAddPhysicDamage() / 100f));
				}
				return __weapon;
			}
		}

		[HarmonyPatch(typeof(Character), "Damage", null)]
		public class VL_Damage_EpicMMOPatch
		{
			public static bool Prefix(Character __instance, ref HitData hit)
			{
				Character attacker = hit.GetAttacker();
				if (__instance.GetSEMan() != null && __instance.GetSEMan().HaveStatusEffect("SE_VL_Charm"))
				{
					//Debug.Log($"Charmed hits: {__instance.GetMaxHealth()}, chance: {100f * ((EpicMMOSystem.LevelSystem.Instance.getLevel() * Mathf.Clamp(EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage(), 0.5f, 100f) * 0.2f) / __instance.GetMaxHealth())}%!");
					if ((hit.m_damage.GetTotalDamage() > 0) && (Random.value > ((EpicMMOSystem.LevelSystem.Instance.getLevel() * Mathf.Clamp(EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage(), 0.5f, 100f) * 0.2f) / __instance.GetMaxHealth()))) {
						//Debug.Log($"Charm released!");
						SE_Charm sE_Charm = (SE_Charm)__instance.GetSEMan().GetStatusEffect("SE_VL_Charm");
						__instance.m_faction = sE_Charm.originalFaction;
						__instance.GetSEMan().RemoveStatusEffect(sE_Charm, quiet: true);
					}
				}
				if (attacker.GetSEMan() != null && attacker.GetSEMan().HaveStatusEffect("SE_VL_Charm"))
				{
					//Debug.Log($"Charmed attacks: {attacker.GetMaxHealth()}, chance: {100f * ((EpicMMOSystem.LevelSystem.Instance.getLevel() * Mathf.Clamp(EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage(), 0.5f, 100f) * 0.2f) / __instance.GetMaxHealth())}%!");
					if ((hit.m_damage.GetTotalDamage() > 0) && (Random.value > ((EpicMMOSystem.LevelSystem.Instance.getLevel() * Mathf.Clamp(EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage(), 0.5f, 100f) * 0.2f) / __instance.GetMaxHealth())))
					{
						//Debug.Log($"Charm released!");
						SE_Charm sE_Charm = (SE_Charm)attacker.GetSEMan().GetStatusEffect("SE_VL_Charm");
						attacker.m_faction = sE_Charm.originalFaction;
						attacker.GetSEMan().RemoveStatusEffect(sE_Charm, quiet: true);
					}
				}
				if (attacker == null)
				{
					if (((int)hit.m_skill == 781))
					{
						hit.m_damage.Modify(1f + (EpicMMOSystem.LevelSystem.Instance.getAddPhysicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
						return true;
					}
					else if (((int)hit.m_skill == 791))
					{
						hit.m_damage.Modify(1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
						return true;
					}
					else if (((int)hit.m_skill == 792))
					{
						hit.m_damage.Modify(1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
						return true;
					}
					else if (((int)hit.m_skill == 793))
					{
						hit.m_damage.Modify(1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
						return true;
					}
					else if (((int)hit.m_skill == 794))
					{
						hit.m_damage.Modify(1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
						return true;
					}
					else if (((int)hit.m_skill == 795))
					{
						hit.m_damage.Modify(1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
						return true;
					}
				} 
				else
				{
					if (__instance.m_name == "Shadow Wolf" && !BaseAI.IsEnemy(__instance, attacker))
					{
						hit.m_damage.Modify(1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
					}
					if (attacker.GetSEMan().HaveStatusEffect("SE_VL_Weaken"))
					{
						SE_Weaken sE_Weaken = (SE_Weaken)attacker.GetSEMan().GetStatusEffect("SE_VL_Weaken");
						hit.m_damage.Modify(1f - ((EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 500f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 400f)));
					}
					if (attacker.GetSEMan().HaveStatusEffect("SE_VL_Monk"))
					{
						SE_Monk sE_Monk = (SE_Monk)attacker.GetSEMan().GetStatusEffect("SE_VL_Monk");
						if (Class_Monk.PlayerIsUnarmed && hit.m_damage.m_blunt > 0f && ((int)hit.m_skill == 11))
						{
							hit.m_damage.m_blunt *= (1f + (EpicMMOSystem.LevelSystem.Instance.getAddPhysicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
						}
					}
					if (attacker.GetSEMan().HaveStatusEffect("SE_VL_Shell"))
					{
						SE_Shell sE_Shell = attacker.GetSEMan().GetStatusEffect("SE_VL_Shell") as SE_Shell;
						hit.m_damage.m_spirit *= (1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
					}
					if (attacker.GetSEMan().HaveStatusEffect("SE_VL_BiomeMist"))
					{
						SE_BiomeMist sE_BiomeMist = attacker.GetSEMan().GetStatusEffect("SE_VL_BiomeMist") as SE_BiomeMist;
						hit.m_damage.m_frost *= (1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
					}
					if (attacker.GetSEMan().HaveStatusEffect("SE_VL_BiomeAsh"))
					{
						SE_BiomeAsh sE_BiomeAsh = attacker.GetSEMan().GetStatusEffect("SE_VL_BiomeAsh") as SE_BiomeAsh;
						hit.m_damage.m_fire *= (1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
					}
					if (attacker.GetSEMan().HaveStatusEffect("SE_VL_Companion"))
					{
						SE_Companion sE_Companion = attacker.GetSEMan().GetStatusEffect("SE_VL_Companion") as SE_Companion;
						hit.m_damage.Modify(1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
					}
					if (attacker.GetSEMan().HaveStatusEffect("SE_VL_RootsBuff"))
					{
						SE_RootsBuff sE_RootsBuff = attacker.GetSEMan().GetStatusEffect("SE_VL_RootsBuff") as SE_RootsBuff;
						hit.m_damage.Modify(1f + (EpicMMOSystem.LevelSystem.Instance.getAddMagicDamage() / 100f) + (EpicMMOSystem.LevelSystem.Instance.getLevel() / 30f));
					}
				}

				return true;
			}

		}

	}

}