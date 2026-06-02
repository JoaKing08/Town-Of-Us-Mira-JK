using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Buttons;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Roles.Impostor;

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
    }
    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        if (OptionGroupSingleton<DemagogueOptions>.Instance.CanBeKilledCrew && @event.Source.Is(ModdedRoleTeams.Crewmate) && !@event.Source.HasModifier<AllianceGameModifier>())
        {
            return;
        }
        if (OptionGroupSingleton<DemagogueOptions>.Instance.CanBeKilledNonCrew && (!@event.Source.Is(ModdedRoleTeams.Crewmate) || @event.Source.HasModifier<AllianceGameModifier>()))
        {
            return;
        }
        if (@event.Target.Data.Role is not DemagogueRole role
            || !role.ImmunityAlive || @event.Source.HasModifier<DemagogueImmunityModifier>(x => x.OwnerId == @event.Target.PlayerId))
        {
            return;
        }
        @event.Cancel();
    }
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;

        if (target == null || button == null || button is not IKillButton || !button.CanClick())
        {
            return;
        }

        if (OptionGroupSingleton<DemagogueOptions>.Instance.CanBeKilledCrew && PlayerControl.LocalPlayer.Is(ModdedRoleTeams.Crewmate) && !PlayerControl.LocalPlayer.HasModifier<AllianceGameModifier>())
        {
            return;
        }
        if (OptionGroupSingleton<DemagogueOptions>.Instance.CanBeKilledNonCrew && (!PlayerControl.LocalPlayer.Is(ModdedRoleTeams.Crewmate) || PlayerControl.LocalPlayer.HasModifier<AllianceGameModifier>()))
        {
            return;
        }
        if (target.Data.Role is not DemagogueRole role
            || !role.ImmunityAlive || PlayerControl.LocalPlayer.HasModifier<DemagogueImmunityModifier>(x => x.OwnerId == target.PlayerId))
        {
            return;
        }
        @event.Cancel();
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