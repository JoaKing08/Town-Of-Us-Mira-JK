using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Options;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Impostor;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Patches
{
    public static class PlayerRoleTextExtensions
    {

        private static Func<BakerFedModifier, bool> BakerPredicate { get; } =
            brModifier => brModifier.BakerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<TavernKeeperDrunkModifier, bool> TavernKeeperPredicate { get; } =
            tkModifier => tkModifier.TavernKeeperId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<BodyguardGuardModifier, bool> BodyguardPredicate { get; } =
            bgModifier => bgModifier.Bodyguard.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<CrusaderFortifyModifier, bool> CrusaderPredicate { get; } =
            crModifier => crModifier.Crusader.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<GunslingerAimedModifier, bool> GunslingerPredicate { get; } =
            crModifier => crModifier.Gunslinger.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<WitchMarkModifier, bool> WitchPredicate { get; } =
            crModifier => crModifier.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<PoisonerPoisonModifier, bool> PoisonerPredicate { get; } =
            crModifier => crModifier.Poisoner.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<DemagogueImmunityModifier, bool> DemagoguePredicate { get; } =
            diModifier => diModifier.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId || PlayerControl.LocalPlayer.IsImpostorAligned();

        [HarmonyPatch]
        public static class UpdateTargetSymbols
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateTargetSymbols), new Type[] { typeof(string), typeof(PlayerControl), typeof(DataVisibility) })]
            [HarmonyPostfix]
            public static void Postfix(PlayerControl player, DataVisibility visibility, ref string __result)
            {
                var hidden = visibility == DataVisibility.Hidden;
                var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                var isDead = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden;

                if (player.TryGetModifier<ManhunterTargetModifier>(out var mhmod) && (PlayerControl.LocalPlayer.IsRole<ManhunterRole>() || isDead))
                {
                    var color = Colors.Manhunter;
                    if (!mhmod.KilledByManhunter && player.HasDied())
                    {
                        color = Color.Lerp(Colors.Manhunter, Color.red, 0.75f);
                    }
                    __result += $"<color=#{color.ToHtmlStringRGBA()}> /</color>";
                }
            }
        }

        [HarmonyPatch]
        public static class UpdateAllianceSymbols
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateAllianceSymbols), new Type[] { typeof(string), typeof(PlayerControl), typeof(DataVisibility) })]
            [HarmonyPostfix]
            public static void Postfix(PlayerControl player, DataVisibility visibility, ref string __result)
            {
                var hidden = visibility == DataVisibility.Hidden;
                var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                var isDead = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden;
                if (player.IsCrewmate() && player.TryGetModifier<ProphetModifier>(out var prophetMod) && (ProphetModifier.ProphetVisibilityFlag(player) || isDead))
                {
                    __result += $"<color=#FFFFFF> (<color=#{Colors.Apocalypse.ToHtmlStringRGBA()}>{prophetMod.ShortName}</color>)</color>";
                }
                if (player.TryGetModifier<NecromancerUndeadModifier>(out var undeadMod) && (NecromancerUndeadModifier.UndeadVisibilityFlag(player) || isDead))
                {
                    __result += $"<color=#FFFFFF> (<color=#{Colors.Necromancer.ToHtmlStringRGBA()}>{undeadMod.ShortName}</color>)</color>";
                }
                if (player.Is((RoleTypes)RoleId.Get<UndercoverRole>()) && isDead)
                {
                    __result += $"<color=#{Colors.Undercover.ToHtmlStringRGBA()}> €</color>";
                }
            }
        }

        [HarmonyPatch]
        public static class UpdateStatusSymbols
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateStatusSymbols), new Type[] { typeof(string), typeof(PlayerControl), typeof(DataVisibility) })]
            [HarmonyPostfix]
            public static void Postfix(PlayerControl player, DataVisibility visibility, ref string __result)
            {
                var hidden = visibility == DataVisibility.Hidden;
                var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                var isImp = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.IsImpostor() && genOpt.ImpsKnowRoles && !genOpt.FFAImpostorMode;
                var isDead = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden;

                if (player.HasModifier(BakerPredicate) && (PlayerControl.LocalPlayer.IsRole<BakerRole>() || PlayerControl.LocalPlayer.IsRole<FamineRole>())
                    || player.HasModifier<BakerFedModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Baker.ToHtmlStringRGBA()}> ";
                    for (int i = 0; i < player.GetModifier<BakerFedModifier>().BreadLeft; i++)
                    {
                        __result += "ß";
                    }
                    __result += "</color>";
                }
                if (player.HasModifier(TavernKeeperPredicate) && PlayerControl.LocalPlayer.IsRole<TavernKeeperRole>()
                    || player.HasModifier<TavernKeeperDrunkModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.TavernKeeper.ToHtmlStringRGBA()}> ø</color>";
                }
                if (player.HasModifier(GunslingerPredicate) && PlayerControl.LocalPlayer.IsRole<GunslingerRole>()
                    || player.HasModifier<GunslingerAimedModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Gunslinger.ToHtmlStringRGBA()}> ‡</color>";
                }
                if (player.HasModifier(WitchPredicate) && PlayerControl.LocalPlayer.IsRole<WitchRole>()
                    || player.HasModifier<WitchMarkModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Witch.ToHtmlStringRGBA()}> ð</color>";
                }
                if (player.HasModifier(PoisonerPredicate) && PlayerControl.LocalPlayer.IsRole<PoisonerRole>()
                    || player.HasModifier<PoisonerPoisonModifier>() && isDead)
                {
                    __result += $"<color=#{TownOfUsColors.Impostor.ToHtmlStringRGBA()}> ž</color>";
                }
                if (player.HasModifier(DemagoguePredicate)
                    || player.HasModifier<DemagogueImmunityModifier>() && isDead)
                {
                    __result += $"<color=#{TownOfUsColors.Impostor.ToHtmlStringRGBA()}> Œ</color>";
                }
            }
        }

        [HarmonyPatch]
        public static class UpdateTargetColor
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateTargetColor), new Type[] { typeof(Color), typeof(PlayerControl), typeof(DataVisibility) })]
            [HarmonyPostfix]
            public static void Postfix(Color color, PlayerControl player, DataVisibility visibility, ref Color __result)
            {
                if (PlayerControl.LocalPlayer.IsImpostorAligned() && player.Is((RoleTypes)RoleId.Get<UndercoverRole>()))
                {
                    __result = Palette.ImpostorRed;
                }
            }
        }

        [HarmonyPatch]
        public static class UpdateProtectionSymbols
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateProtectionSymbols), new Type[] { typeof(string), typeof(PlayerControl), typeof(DataVisibility) })]
            [HarmonyPostfix]
            public static void Postfix(PlayerControl player, DataVisibility visibility, ref string __result)
            {
                var hidden = visibility == DataVisibility.Hidden;
                var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                var isDead = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden;
                if (player.HasModifier(BodyguardPredicate) && PlayerControl.LocalPlayer.IsRole<BodyguardRole>()
                    || player.HasModifier<BodyguardGuardModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Bodyguard.ToHtmlStringRGBA()}> ()</color>";
                }
                if (player.HasModifier(CrusaderPredicate) && PlayerControl.LocalPlayer.IsRole<CrusaderRole>()
                    || player.HasModifier<CrusaderFortifyModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Crusader.ToHtmlStringRGBA()}> {{}}</color>";
                }
            }
        }
    }
}
