using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modifiers.Game.Neutral;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.GameOptions
{
    public static class CustomAlignmentsData
    {
        public static RoleOptionsGroup NeutralApocalypse { get; } = new("Neutral Apocalypse Roles", Colors.Apocalypse);
    }
    [HarmonyPatch]
    public static class CustomAlignments
    {
        // To not patch a million methods to include apocalypse.
        [HarmonyPatch(typeof(TownOfUs.Utilities.Extensions), "Is", [typeof(PlayerControl), typeof(RoleAlignment)])]
        [HarmonyPostfix]
        public static void IsAlignment(PlayerControl player, RoleAlignment roleAlignment, ref bool __result)
        {
            if (roleAlignment == RoleAlignment.NeutralKilling && player.Data.Role.GetRoleAlignment() == (RoleAlignment)27)
            {
                __result = true;
            }
        }

        [HarmonyPatch(typeof(TownOfUsRoleButton<SheriffRole, PlayerControl>), nameof(TownOfUsRoleButton<SheriffRole, PlayerControl>.Enabled))]
        [HarmonyPrefix]
        public static bool SheriffButtonEnabled(RoleBehaviour? role, TownOfUsRoleButton<SheriffRole, PlayerControl> __instance, ref bool __result)
        {
            try
            {
                __result = __instance?.Disabled != true && role != null && role?.Role == __instance?.Role?.Role && __instance != CustomButtonSingleton<SheriffShootButton>.Instance;
            }
            catch
            {
                __result = false;
            }
            return false;
        }

        [HarmonyPatch(typeof(AssassinModifier), "IsRoleValid")]
        [HarmonyPostfix]
        public static void IsRoleValid(RoleBehaviour role, AssassinModifier __instance, ref bool __result)
        {
            if (!role.IsDead && role is not IGhostRole && role is not IUnguessable { IsGuessable: false } && role.GetRoleAlignment() == (RoleAlignment)27 && __instance.Player.Data.Role.GetRoleAlignment() != (RoleAlignment)27)
            {
                __result = OptionGroupSingleton<AssassinOptions>.Instance.AssassinGuessNeutralKilling.Value;
            }
        }

        [HarmonyPatch(typeof(NeutralKillerAssassinModifier), nameof(NeutralKillerAssassinModifier.IsModifierValidOn))]
        [HarmonyPostfix]
        public static void IsModifierValidOn(RoleBehaviour role, NeutralKillerAssassinModifier __instance, ref bool __result)
        {
            __result |= role is ITownOfUsRole { RoleAlignment: (RoleAlignment)27 };
        }

        [HarmonyPatch(typeof(NeutralKillerDoubleShotModifier), nameof(NeutralKillerDoubleShotModifier.IsModifierValidOn))]
        [HarmonyPostfix]
        public static void IsModifierValidOn(RoleBehaviour role, NeutralKillerDoubleShotModifier __instance, ref bool __result)
        {
            __result |= role is ITownOfUsRole { RoleAlignment: (RoleAlignment)27 }
            && role.Player.GetModifierComponent().HasModifier<NeutralKillerAssassinModifier>(true)
            && !role.Player.GetModifierComponent().HasModifier<TouGameModifier>(true);
        }

        [HarmonyPatch(typeof(SnitchRole), "CreateSnitchArrows")]
        [HarmonyPostfix]
        public static void CreateSnitchArrows(bool silent, SnitchRole __instance)
        {
            var _snitchArrows = (Dictionary<byte, ArrowBehaviour>?)typeof(SnitchRole).GetField("_snitchArrows").GetValue(__instance);
            if (OptionGroupSingleton<SnitchOptions>.Instance.SnitchNeutralRoles)
            {
                var neutrals = MiscUtils.GetRoles((RoleAlignment)27)
                    .Where(role => !role.Player.Data.IsDead && !role.Player.Data.Disconnected);
                neutrals.ToList().ForEach(neutral =>
                {
                    _snitchArrows.Add(neutral.Player.PlayerId,
                        MiscUtils.CreateArrow(neutral.Player.transform, TownOfUsColors.Neutral));
                    PlayerNameColor.Set(neutral.Player);
                    neutral.Player.AddModifier<SnitchImpostorRevealModifier>();
                });
            }
        }

        [HarmonyPatch(typeof(VigilanteRole), "IsRoleValid")]
        [HarmonyPostfix]
        public static void IsRoleValid(RoleBehaviour role, VigilanteRole __instance, ref bool __result)
        {
            if (!role.IsDead && role is not IUnguessable { IsGuessable: false } && role.GetRoleAlignment() == (RoleAlignment)27)
            {
                __result = OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteGuessNeutralKilling.Value;
            }
        }
    }
}
