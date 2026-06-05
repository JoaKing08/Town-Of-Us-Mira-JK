using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using System.Reflection;
using TownOfUs.Assets;
using TownOfUs.Buttons.Impostor;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Impostor.Herbalist;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Options.Maps;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Patches.Options;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Impostor;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Modifiers.Game.Impostor;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using UnityEngine;

namespace TownOfUs.Patches.Roles;

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
[HarmonyPriority(Priority.First)]
public static class ImpostorTargetPatch
{
    public static bool Prefix(ImpostorRole __instance, [HarmonyArgument(0)] NetworkedPlayerInfo target,
        ref bool __result)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var saboOpt = OptionGroupSingleton<AdvancedSabotageOptions>.Instance;
        var loveOpt = OptionGroupSingleton<LoversOptions>.Instance;
        var isValid = target is { Disconnected: false, IsDead: false } && target.PlayerId != __instance.Player.PlayerId && target.Role && target.Object && !target.Object.inVent && !target.Object.inMovingPlat;
        if (!isValid)
        {
            __result = false;
            return false;
        }
        var isImp = target.Object.IsImpostorAligned() || target.Object.Is((RoleTypes)RoleId.Get<UndercoverRole>());
        var killImps = (OptionGroupSingleton<UndercoverOptions>.Instance.ImpsKillEachother && UndercoverRole.InPlay) || genOpt.FFAImpostorMode || (PlayerControl.LocalPlayer.IsLover() && loveOpt.LoverKillTeammates) || __instance.Player.HasModifier<OutcastModifier>() || target.Object.HasModifier<OutcastModifier>();
        var killAnyone = saboOpt.KillDuringCamoComms && target.Object.GetAppearanceType() == TownOfUsAppearances.Camouflage;
        var isLover = !loveOpt.LoversKillEachOther && target.Object.IsLover() && PlayerControl.LocalPlayer.IsLover();
        var isUnkillable = (target.Object.TryGetModifier<DisabledModifier>(out var mod) && !mod.CanBeInteractedWith);

        __result = !isUnkillable && (!isImp || killImps || killAnyone) && (!isLover || killAnyone);
        return false;
    }
}

[HarmonyPatch(typeof(ImpostorValidTargetPatch), nameof(ImpostorValidTargetPatch.Prefix))]
[HarmonyPatch(typeof(ImpostorTargeting), nameof(ImpostorTargeting.Postfix))]
[HarmonyPriority(Priority.First)]
public static class StopOtherKillButtonPatches
{
    public static bool Prefix()
    {
        return false;
    }
}

[HarmonyPatch(typeof(ParasiteOvertakeButton), nameof(ParasiteOvertakeButton.GetTarget))]
public static class GetTargetParasite
{
    public static bool Prefix(ParasiteOvertakeButton __instance, ref PlayerControl? __result)
    {
        if (PlayerControl.LocalPlayer.Data?.Role is not ParasiteRole pr)
        {
            __result = null;
            return false;
        }

        if (PlayerControl.LocalPlayer.Data?.Role.Cast<ParasiteRole>().Controlled != null)
        {
            __result = PlayerControl.LocalPlayer.Data?.Role.Cast<ParasiteRole>().Controlled;
            return false;
        }

        __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(
            true,
            __instance.Distance,
            predicate: plr =>
                plr != null &&
                plr != PlayerControl.LocalPlayer &&
                !plr.HasDied() &&
                !((plr.IsImpostorAligned() || plr.Is((RoleTypes)RoleId.Get<UndercoverRole>())) &&
                !(OptionGroupSingleton<UndercoverOptions>.Instance.ImpsKillEachother && UndercoverRole.InPlay) &&
                !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>() && !plr.HasModifier<OutcastModifier>()) &&
                !plr.IsInTargetingAnimState() &&
                !plr.GetModifiers<BaseModifier>().Any(x => x is IUncontrollable) &&
                !plr.HasModifier<ParasiteInfectedModifier>());
        return false;
    }
}

[HarmonyPatch(typeof(HerbalistAbilityHerbButton), nameof(HerbalistAbilityHerbButton.GetTarget))]
public static class GetTargetHerbalist
{
    private static Func<HerbalistExposedModifier, bool> ExposedPredicate { get; } =
        msModifier => msModifier.Herbalist.AmOwner;

    private static Func<HerbalistConfusedModifier, bool> ConfusedPredicate { get; } =
        msModifier => msModifier.Herbalist.AmOwner;

    private static Func<HerbalistProtectionModifier, bool> ProtectedPredicate { get; } =
        msModifier => msModifier.Herbalist.AmOwner;
    public static bool Prefix(HerbalistAbilityHerbButton __instance, ref PlayerControl? __result)
    {
        var isFfa = OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode || (OptionGroupSingleton<UndercoverOptions>.Instance.ImpsKillEachother && UndercoverRole.InPlay) || PlayerControl.LocalPlayer.HasModifier<OutcastModifier>();
        if (__instance.CurrentAbility is HerbAbilities.Expose)
        {
            __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, false, x => (isFfa || !(x.IsImpostorAligned() || x.Is((RoleTypes)RoleId.Get<UndercoverRole>())) || x.HasModifier<OutcastModifier>()) && !x.HasModifier(ExposedPredicate));
            return false;
        }
        if (__instance.CurrentAbility is HerbAbilities.Confuse)
        {
            __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, false, x => (isFfa || !(x.IsImpostorAligned() || x.Is((RoleTypes)RoleId.Get<UndercoverRole>())) || x.HasModifier<OutcastModifier>()) && !x.HasModifier(ConfusedPredicate));
            return false;
        }
        if (__instance.CurrentAbility is HerbAbilities.Protect)
        {
            __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, false, x => !x.HasModifier(ProtectedPredicate));
            return false;
        }
        __result = MiscUtils.GetImpostorTarget(__instance.Distance);
        return false;
    }
}

[HarmonyPatch(typeof(PuppeteerKillButton), nameof(PuppeteerKillButton.GetTarget))]
public static class GetTargetPuppeter
{
    public static bool Prefix(PuppeteerKillButton __instance, ref PlayerControl? __result)
    {
        if (CustomButtonSingleton<PuppeteerControlButton>.Instance.EffectActive && __instance.Role.Controlled != null)
        {
            __result = __instance.Role.Controlled.GetClosestLivingPlayer(
                true,
                __instance.Distance,
                predicate: plr =>
                    plr != null &&
                    plr != PlayerControl.LocalPlayer &&
                    !plr.HasDied() &&
                    !plr.IsInTargetingAnimState() &&
                    !((plr.IsImpostorAligned() || plr.Is((RoleTypes)RoleId.Get<UndercoverRole>())) &&
                    !(OptionGroupSingleton<UndercoverOptions>.Instance.ImpsKillEachother && UndercoverRole.InPlay) &&
                !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>() && !plr.HasModifier<OutcastModifier>()));
            return false;
        }
        __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(
            false,
            __instance.Distance,
            predicate: plr => plr != null && !plr.HasDied() && !plr.IsInTargetingAnimState());
        return false;
    }
}

[HarmonyPatch(typeof(SpellslingerHexButton), nameof(SpellslingerHexButton.GetTarget))]
public static class GetTargetSpellslinger
{
    public static bool Prefix(SpellslingerHexButton __instance, ref PlayerControl? __result)
    {
        if (UndercoverRole.InPlay)
        {
            __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, predicate: x => !x.HasModifier<SpellslingerHexedModifier>());
        }
        else
        {
            __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, predicate: x => !x.HasModifier<SpellslingerHexedModifier>() && (!x.IsImpostor() || x.HasModifier<OutcastModifier>() || PlayerControl.LocalPlayer.HasModifier<OutcastModifier>()));
        }
        return false;
    }
}

[HarmonyPatch(typeof(HypnotistHypnotizeButton), nameof(HypnotistHypnotizeButton.GetTarget))]
public static class GetTargetHypnotist
{
    public static bool Prefix(HypnotistHypnotizeButton __instance, ref PlayerControl? __result)
    {
        if (UndercoverRole.InPlay)
        {
            __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, predicate: x => !x.HasModifier<HypnotisedModifier>());
        }
        else
        {
            __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, predicate: x => !x.HasModifier<HypnotisedModifier>() && (!x.IsImpostor() || x.HasModifier<OutcastModifier>() || PlayerControl.LocalPlayer.HasModifier<OutcastModifier>()));
        }
        return false;
        return false;
    }
}

[HarmonyPatch(typeof(WarlockKillButton), nameof(WarlockKillButton.GetTarget))]
public static class GetTargetWarlock
{
    public static bool Prefix(WarlockKillButton __instance, ref PlayerControl? __result)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var saboOpt = OptionGroupSingleton<AdvancedSabotageOptions>.Instance;
        var closePlayer = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance);

        var includePostors = genOpt.FFAImpostorMode ||
                             (PlayerControl.LocalPlayer.IsLover() &&
                              OptionGroupSingleton<LoversOptions>.Instance.LoverKillTeammates) ||
                             (saboOpt.KillDuringCamoComms &&
                              closePlayer?.GetAppearanceType() == TownOfUsAppearances.Camouflage) ||
                              (OptionGroupSingleton<UndercoverOptions>.Instance.ImpsKillEachother && UndercoverRole.InPlay) ||
                              closePlayer?.HasModifier<OutcastModifier>() == true || PlayerControl.LocalPlayer.HasModifier<OutcastModifier>();
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, false,
                x => !__instance.MarkedTargets.Contains(x) && (!x.IsLover() || !((x.IsImpostorAligned() || x.IsRole<UndercoverRole>()) && !includePostors)));
            return false;
        }

        __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, __instance.Distance, false,
            x => !__instance.MarkedTargets.Contains(x) && !((x.IsImpostorAligned() || x.IsRole<UndercoverRole>()) && !includePostors));
        return false;
    }
}

[HarmonyPatch(typeof(MiscUtils), nameof(MiscUtils.GetImpostorTarget))]
public static class GetImpostorTarget
{
    public static bool Prefix(float distance, ref PlayerControl? __result)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var saboOpt = OptionGroupSingleton<AdvancedSabotageOptions>.Instance;
        var closePlayer = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, distance);

        var includePostors = genOpt.FFAImpostorMode ||
                             (PlayerControl.LocalPlayer.IsLover() &&
                              OptionGroupSingleton<LoversOptions>.Instance.LoverKillTeammates) ||
                             (saboOpt.KillDuringCamoComms &&
                              closePlayer?.GetAppearanceType() == TownOfUsAppearances.Camouflage) ||
                              (OptionGroupSingleton<UndercoverOptions>.Instance.ImpsKillEachother && UndercoverRole.InPlay) ||
                              closePlayer?.HasModifier<OutcastModifier>() == true || PlayerControl.LocalPlayer.HasModifier<OutcastModifier>();
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, distance, false, x => !x.IsLover() || !((x.IsImpostorAligned() || x.IsRole<UndercoverRole>()) && !includePostors));
            return false;
        }

        __result = PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, distance, false, x => !((x.IsImpostorAligned() || x.IsRole<UndercoverRole>()) && !includePostors));
        return false;
    }
}

[HarmonyPatch(typeof(AssassinModifier), nameof(AssassinModifier.IsExempt))]
public static class IsAssassinExempt
{
    public static bool Prefix(PlayerVoteArea voteArea, AssassinModifier __instance, ref bool __result)
    {
        var votePlayer = voteArea.GetPlayer();
        __result = voteArea?.TargetPlayerId == __instance.Player.PlayerId ||
               __instance.Player.Data.IsDead ||
               voteArea!.AmDead ||
               (__instance.Player.IsImpostorAligned() && (votePlayer?.IsImpostorAligned() == true || votePlayer?.Is((RoleTypes)RoleId.Get<UndercoverRole>()) == true) &&
               !OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode && !(UndercoverRole.InPlay && OptionGroupSingleton<UndercoverOptions>.Instance.ImpsKillEachother) &&
               !votePlayer.HasModifier<OutcastModifier>() && !__instance.Player.HasModifier<OutcastModifier>()) ||
               (__instance.Player.Data.Role is VampireRole && votePlayer?.Data.Role is VampireRole) ||
               (votePlayer?.Data.Role is MayorRole mayor && mayor.Revealed) ||
               votePlayer.IsRevealed() ||
               (__instance.Player.IsLover() && votePlayer?.IsLover() == true) ||
               votePlayer?.HasModifier<JailedModifier>() == true;
        return false;
    }
}

[HarmonyPatch(typeof(AmbassadorRole), "IsExempt")]
public static class IsAmbassadorExempt
{
    public static bool Prefix(PlayerVoteArea voteArea, AmbassadorRole __instance, ref bool __result)
    {
        __result = __instance.Player.Data.IsDead || voteArea.AmDead || (voteArea.GetPlayer()?.IsImpostor() == false && voteArea.GetPlayer()?.IsRole<UndercoverRole>() == false) || voteArea.GetPlayer()?.HasModifier<OutcastModifier>() == true || voteArea.GetPlayer()?.IsRole<MafiosoRole>() == true ||
               voteArea.GetPlayer()?.HasModifier<AmbassadorRetrainedModifier>() == true
               || OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode && !__instance.Player.AmOwner;
        return false;
    }
}

[HarmonyPatch(typeof(AmbassadorRole), nameof(AmbassadorRole.Click))]
public static class AmbassadorClick
{
    public static MeetingMenu? GetMeetingMenu(AmbassadorRole x) => typeof(AmbassadorRole).GetField("meetingMenu", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(x) as MeetingMenu;
    public static void RpcRetrain(AmbassadorRole x, PlayerControl player, byte playerId = byte.MaxValue, ushort role = 0) => typeof(AmbassadorRole).GetMethod("RpcRetrain", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(x, [player, playerId, role]);
    public static bool Prefix(PlayerVoteArea voteArea, AmbassadorRole __instance)
    {
        var meetingMenu = GetMeetingMenu(__instance);
        var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId);

        if (__instance.SelectedPlr == player)
        {
            RpcRetrain(__instance, PlayerControl.LocalPlayer);
            meetingMenu!.Actives[voteArea.TargetPlayerId] = false;
            return false;
        }

        if (__instance.SelectedPlr != null)
        {
            meetingMenu!.Actives[voteArea.TargetPlayerId] = false;
            meetingMenu!.Actives[__instance.SelectedPlr.PlayerId] = false;
            RpcRetrain(__instance, PlayerControl.LocalPlayer);
        }

        var opt = OptionGroupSingleton<AmbassadorOptions>.Instance;
        if ((int)opt.KillsNeeded > 0)
        {
            var killedAmbassPlayers = GameHistory.KilledPlayers.Count(x =>
                x.KillerId == __instance.Player.PlayerId && x.VictimId != __instance.Player.PlayerId);

            var killedPlayerPlayers = GameHistory.KilledPlayers.Count(x =>
                x.KillerId == voteArea.GetPlayer()?.PlayerId && x.VictimId != voteArea.GetPlayer()?.PlayerId);

            if (killedAmbassPlayers < (int)opt.KillsNeeded && killedPlayerPlayers < (int)opt.KillsNeeded)
            {
                var text =
                    TouLocale.GetParsed("TouRoleAmbassadorNeedKills")
                        .Replace("<requiredKills>", $"{(int)opt.KillsNeeded}");
                var notif1 =
                    Helpers.CreateAndShowNotification(text, Color.white, new Vector3(0f, 1f, -20f),
                        spr: TouRoleIcons.Ambassador.LoadAsset());

                notif1.AdjustNotification();
                return false;
            }
        }

        var excluded = MiscUtils.AllRegisteredRoles
            .Where(x => x is ISpawnChange { NoSpawn: true } || x.Role is RoleTypes.Impostor || x.IsDead || x is ITownOfUsRole
            {
                RoleAlignment: RoleAlignment.ImpostorPower
            }).Select(x => x.Role).ToList();
        var impRoles = MiscUtils.GetRolesToAssign(ModdedRoleTeams.Impostor, x => !excluded.Contains(x.Role))
            .Select(x => x.RoleType).ToList();

        foreach (var player2 in PlayerControl.AllPlayerControls)
        {
            if ((player2.IsImpostor() || player2.IsRole<UndercoverRole>()) && !player2.AmOwner)
            {
                var role = player2.GetRoleWhenAlive();
                if (role)
                {
                    impRoles.Remove((ushort)role!.Role);
                }
                if (role is UndercoverRole && player2.TryGetModifier<UndercoverCoverModifier>(out var cover) && cover.ShownRole)
                {
                    impRoles.Remove((ushort)cover.ShownRole!.Role);
                }

                if (player2.TryGetModifier<AmbassadorRetrainedModifier>(out var retrained))
                {
                    impRoles.Remove((ushort)retrained.PreviousRole.Role);
                }
            }
        }

        var roleList = MiscUtils.GetPotentialRoles()
            .Where(role => impRoles.Contains((ushort)role.Role))
            .ToList();

        if (TutorialManager.InstanceExists)
        {
            impRoles = MiscUtils.GetRegisteredRoles(ModdedRoleTeams.Impostor)
                .Where(x => !excluded.Contains(x.Role))
                .Select(x => (ushort)x.Role).ToList();
            roleList = MiscUtils.AllRegisteredRoles
                .Where(role => impRoles.Contains((ushort)role.Role))
                .ToList();
        }

        if (!player._object.Is(RoleAlignment.ImpostorKilling) && !player._object.Is(RoleAlignment.ImpostorPower) &&
            player._object.GetModifier<UndercoverCoverModifier>()?.ShownRole?.GetRoleAlignment() != RoleAlignment.ImpostorKilling &&
            player._object.GetModifier<UndercoverCoverModifier>()?.ShownRole?.GetRoleAlignment() != RoleAlignment.ImpostorPower)
        {
            var curRoleList = MiscUtils.GetPotentialRoles()
                .Where(role => impRoles.Contains(RoleId.Get(role.GetType())))
                .ToList();

            if (TutorialManager.InstanceExists)
            {
                impRoles = MiscUtils.GetRegisteredRoles(ModdedRoleTeams.Impostor)
                    .Where(x => !excluded.Contains(x.Role))
                    .Select(x => (ushort)x.Role).ToList();
                curRoleList = MiscUtils.AllRegisteredRoles
                    .Where(role => impRoles.Contains(RoleId.Get(role.GetType())))
                    .ToList();
            }
            foreach (var roleBehaviour in curRoleList)
            {
                if (roleBehaviour.GetRoleAlignment() == RoleAlignment.ImpostorKilling)
                {
                    roleList.Remove(roleBehaviour);
                }
            }
        }

        if (!Minigame.Instance)
        {
            var trainMenu = AmbassadorSelectionMinigame.Create();
            trainMenu.Open(
                roleList,
                role =>
                {
                    if (role != null)
                    {
                        meetingMenu.Actives[voteArea.TargetPlayerId] = true;
                        RpcRetrain(__instance, PlayerControl.LocalPlayer, player.PlayerId, (ushort)role.Role);
                    }

                    trainMenu.Close();
                }
            );
        }
        return false;
    }
}

/*[HarmonyPatch(typeof(Utilities.Extensions), nameof(Utilities.Extensions.ChangeRole))]
public static class UndercoverChangeRole
{
    public static bool Prefix(PlayerControl player, ushort newRoleType, bool recordRole)
    {
        if (player?.IsRole<UndercoverRole>() == true && RoleManager.Instance?.GetRole((RoleTypes)newRoleType)?.IsImpostor() == true && newRoleType != RoleId.Get<TraitorRole>() && newRoleType != RoleId.Get<MafiosoRole>())
        {
            player.GetModifier<UndercoverCoverModifier>().ShownRole = RoleManager.Instance.GetRole((RoleTypes)newRoleType);
            return false;
        }
        return true;
    }
}*/