using AmongUs.GameOptions;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Modifiers.Crewmate;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Modules.Components;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;
using static Rewired.Demos.CustomPlatform.MyPlatformControllerExtension;

namespace TownOfUsMiraJK.Events.Impostor;

public static class DemagogueEvents
{
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        if (DemagogueRevealModifier.Tint != null)
        {
            DemagogueRevealModifier.Tint.gameObject.SetActive(false);
        }

        foreach (var player in ModifierUtils.GetPlayersWithModifier<DemagoguePunishModifier>())
        {
            DeathHandlerModifier.UpdateDeathHandlerImmediate(player, TouLocale.Get("DiedToDemagogue"),
                DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                lockInfo: DeathHandlerOverride.SetTrue);
            player.Exiled();
        }
    }

    [RegisterEvent(31)]
    public static void ProcessVotesEventHandler(ProcessVotesEvent @event)
    {
        if (@event.ExiledPlayer == null)
        {
            return;
        }

        if (@event.ExiledPlayer?.Role is not DemagogueRole role || !role.ImmunityAlive)
        {
            return;
        }

        DemagogueRole.RpcDemagogueImmunitized(@event.ExiledPlayer!.Object, UnityEngine.Random.RandomRangeInt(1, 101) <= 10);
        if (OptionGroupSingleton<DemagogueOptions>.Instance.PunishVoters)
        {
            var toPunish = @event.Votes.Where(x => x.Suspect == @event.ExiledPlayer!.PlayerId).Select(x => MiscUtils.PlayerById(x.Voter)).Where(x => x != null && !x.IsImpostorAligned() && !x.HasModifier<DemagogueImmunityModifier>()).ToHashSet().ToList();
            foreach (var punished in toPunish)
            {
                punished.AddModifier<DemagoguePunishModifier>();
            }
        }
        @event.ExiledPlayer = null;
    }
    [RegisterEvent(2)]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (@event.Source.IsRole<DemagogueRole>())
        {
            @event.Source.SetKillTimer(@event.Source.GetKillCooldown() + OptionGroupSingleton<DemagogueOptions>.Instance.KillCooldownDebuff);
        }
        if (!OptionGroupSingleton<DemagogueOptions>.Instance.PunishKillers)
        {
            return;
        }
        if (@event.Target.Data.Role is not DemagogueRole role
            || !role.ImmunityAlive || @event.Source.HasModifier<DemagogueImmunityModifier>(x => x.OwnerId == @event.Target.PlayerId))
        {
            return;
        }
        if ((!@event.Source.IsCrewmate()
            || @event.Source.HasModifier<AllianceGameModifier>()) && !OptionGroupSingleton<DemagogueOptions>.Instance.PunishNonCrew)
        {
            return;
        }
        var showAnim = MeetingHud.Instance == null && ExileController.Instance == null;
        var murderResultFlags2 = MurderResultFlags.DecisionByHost | MurderResultFlags.Succeeded;

        DeathHandlerModifier.UpdateDeathHandlerImmediate(@event.Source, TouLocale.Get("DiedToDemagogue"),
            DeathEventHandlers.CurrentRound,
            (!MeetingHud.Instance && !ExileController.Instance)
                ? DeathHandlerOverride.SetTrue
                : DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
        @event.Target.CustomMurder(
            @event.Source,
            murderResultFlags2,
            false,
            showAnim,
            false,
            showAnim,
            false);
    }

    [RegisterEvent(1)]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!PlayerControl.LocalPlayer.IsRole<DemagogueRole>())
        {
            return;
        }

        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown() + OptionGroupSingleton<DemagogueOptions>.Instance.KillCooldownDebuff);
    }
}