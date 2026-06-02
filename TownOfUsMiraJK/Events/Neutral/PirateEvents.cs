using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Events.Modifiers;

public static class PirateEvents
{
    [RegisterEvent]
    public static void VoteEvent(CheckForEndVotingEvent @event)
    {
        if (!@event.IsVotingComplete || !PlayerControl.LocalPlayer.IsHost())
        {
            return;
        }

        foreach (var pirate in CustomRoleUtils.GetActiveRolesOfType<PirateRole>().Where(x => !x.Player.HasDied()))
        {
            PirateRole.RpcDoDuel(pirate.Player);
        }

        foreach (var player in ModifierUtils.GetPlayersWithModifier<PirateDuelModifier>())
        {
            player.RpcRemoveModifier<PirateDuelModifier>();
        }
    }
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        foreach (var pirate in CustomRoleUtils.GetActiveRolesOfType<PirateRole>())
        {
            if (pirate.MetWinCon && !pirate.Won)
            {
                if (pirate.Player.AmOwner)
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRolePirateVictoryMessageSelf").Replace("<role>", $"{Colors.Pirate.ToTextColor()}{pirate.RoleName}</color>"),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Pirate.LoadAsset());

                    notif1.AdjustNotification();
                }
                else
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRolePirateVictoryMessage").Replace("<player>", pirate.Player.Data.PlayerName).Replace("<role>", $"{Colors.Pirate.ToTextColor()}{pirate.RoleName}</color>"),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Pirate.LoadAsset());

                    notif1.AdjustNotification();
                }
                if (!pirate.Player.HasDied())
                {
                    DeathHandlerModifier.UpdateDeathHandlerImmediate(pirate.Player, TouLocale.Get("DiedToWinning"),
                        DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                        lockInfo: DeathHandlerOverride.SetTrue);

                    pirate.Player.Exiled();
                }
                pirate.Won = true;
            }
        }
    }
}