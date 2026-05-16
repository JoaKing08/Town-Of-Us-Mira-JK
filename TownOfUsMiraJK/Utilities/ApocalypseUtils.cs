using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TownOfUs;
using TownOfUs.Buttons.Neutral;
using TownOfUs.GameOver;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modules.Components;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Utilities
{
    public static class ApocalypseUtils
    {
        public static bool IsApocalypse(this PlayerControl player)
        {
            return player.Data.IsApocalypse();
        }
        public static bool IsApocalypse(this NetworkedPlayerInfo player)
        {
            return player.Role.IsApocalypse();
        }
        public static bool IsApocalypse(this RoleBehaviour role)
        {
            return role.Role == (RoleTypes)RoleId.Get<PlaguebearerJKRole>() || role.Role == (RoleTypes)RoleId.Get<PestilenceJKRole>() || role.Role == (RoleTypes)RoleId.Get<BakerRole>() || role.Role == (RoleTypes)RoleId.Get<FamineRole>() || role.Role == (RoleTypes)RoleId.Get<BerserkerRole>() || role.Role == (RoleTypes)RoleId.Get<WarRole>() || role.Role == (RoleTypes)RoleId.Get<SoulCollectorJKRole>() || role.Role == (RoleTypes)RoleId.Get<DeathRole>();
        }
        public static bool IsApocalypseAligned(this PlayerControl player)
        {
            return player.IsApocalypse() || player.GetModifiers<AllianceGameModifier>().Any(x => x.TrueFactionType is (AlliedFaction)9);
        }
        public static bool IsApocalypseAligned(this NetworkedPlayerInfo player)
        {
            return player.Object.IsApocalypseAligned();
        }
        public static bool IsApocalypseAligned(this RoleBehaviour role)
        {
            return role.Player.IsApocalypseAligned();
        }
        public static bool ApocalypseWinConditionMet()
        {
            if (ArmageddonSabotageSystem.ArmageddonFinished)
            {
                return true;
            }
            var apocalypseMembers = CustomRoleUtils.GetActiveRolesOfTeam(ModdedRoleTeams.Custom).Where(x => x.IsApocalypse() && !x.Player.HasDied());
            var allApocalypseMembers = apocalypseMembers;

            if (MiscUtils.KillersAliveCount > apocalypseMembers.Count())
            {
                return false;
            }
            var winConMet = allApocalypseMembers.Count() >= Helpers.GetAlivePlayers().Count - allApocalypseMembers.Count();
            return winConMet;
        }

        [HarmonyPatch]
        public static class PlaguebearerPatches
        {
            [HarmonyPatch(typeof(PestilenceRole), nameof(PestilenceRole.WinConditionMet))]
            [HarmonyPrefix]
            public static bool WinConditionMetPest(ref bool __result)
            {
                __result = ApocalypseWinConditionMet();
                return false;
            }
            [HarmonyPatch(typeof(PestilenceKillButton), nameof(PestilenceKillButton.GetTarget))]
            [HarmonyPrefix]
            public static bool GetTargetPest(ref PlayerControl? __result, PestilenceKillButton __instance)
            {
                if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
                {
                    __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, false, x => !x.IsLover() && !(x.IsApocalypse() && !OptionGroupSingleton<LoversOptions>.Instance.LoverKillTeammates));
                    return false;
                }

                __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, false, x => !x.IsApocalypse());
                return false;
            }
        }
        [HarmonyPatch]
        public static class EndGamePatch
        {
            [HarmonyPatch(typeof(NeutralGameOver), nameof(NeutralGameOver.AfterEndGameSetup))]
            [HarmonyPrefix]
            public static bool AfterEndGameSetup(NeutralGameOver __instance, EndGameManager endGameManager)
            {
                var _role = (RoleBehaviour?)typeof(NeutralGameOver)?.GetField("_role", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance);
                if (_role?.IsApocalypse() != true)
                {
                    return true;
                }
                endGameManager.BackgroundBar.material.SetColor(ShaderID.Color, Colors.Apocalypse);

                var text = UnityEngine.Object.Instantiate(endGameManager.WinText);
                var winText = TouLocale.GetParsed("TouJKApocalypseWin");
                text.text = $"{winText}!";
                text.color = Colors.Apocalypse;
                GameHistory.WinningFaction = $"<color=#{Colors.Apocalypse.ToHtmlStringRGBA()}>{TouLocale.GetParsed("TouJKApocalypseWin")}</color>";

                var pos = endGameManager.WinText.transform.localPosition;
                pos.y = 1.5f;
                pos += Vector3.down * 0.15f;
                text.transform.localScale = new Vector3(1f, 1f, 1f);

                text.transform.position = pos;
                text.text = $"<size=4>{text.text}</size>";
                return false;
            }
        }
    }
}
