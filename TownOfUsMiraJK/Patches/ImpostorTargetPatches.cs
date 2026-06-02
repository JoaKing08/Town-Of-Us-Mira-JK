using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Buttons.Impostor;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Impostor.Herbalist;
using TownOfUs.Options;
using TownOfUs.Options.Maps;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Patches.Options;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Impostor;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Modifiers.Game.Impostor;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

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
public static class IsExempt
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