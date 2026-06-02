using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJk.Events.Neutral;

public static class WitchEvents
{
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var witch = CustomRoleUtils.GetActiveRolesOfType<WitchRole>().FirstOrDefault();
        if (witch != null && !Helpers.GetAlivePlayers().Any(x => x.IsCrewmate() && !x.HasModifier<AllianceGameModifier>()) && !witch.Player.HasDied())
        {
            if (witch.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKPariahVictoryMessageSelf").Replace("<role>", $"{Colors.Witch.ToTextColor()}{witch.RoleName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Witch.LoadAsset());

                notif1.AdjustNotification();
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKPariahVictoryMessage").Replace("<player>", witch.Player.Data.PlayerName).Replace("<role>", $"{Colors.Witch.ToTextColor()}{witch.RoleName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Witch.LoadAsset());

                notif1.AdjustNotification();
            }
            witch.HasWon = true;
            DeathHandlerModifier.UpdateDeathHandlerImmediate(witch.Player, TouLocale.Get("DiedToWinning"),
                DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                lockInfo: DeathHandlerOverride.SetTrue);

            witch.Player.Exiled();
        }
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }
        var witch = CustomRoleUtils.GetActiveRolesOfType<WitchRole>().FirstOrDefault();
        if (witch != null && !Helpers.GetAlivePlayers().Any(x => x.IsCrewmate() && !x.HasModifier<AllianceGameModifier>()) && !witch.Player.HasDied())
        {
            if (witch.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKPariahVictoryMessageSelf").Replace("<role>", $"{Colors.Witch.ToTextColor()}{witch.RoleName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Witch.LoadAsset());

                notif1.AdjustNotification();
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKPariahVictoryMessage").Replace("<player>", witch.Player.Data.PlayerName).Replace("<role>", $"{Colors.Witch.ToTextColor()}{witch.RoleName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Witch.LoadAsset());

                notif1.AdjustNotification();
            }
            witch.HasWon = true;
            DeathHandlerModifier.UpdateDeathHandlerImmediate(witch.Player, TouLocale.Get("DiedToWinning"),
                DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                lockInfo: DeathHandlerOverride.SetTrue);

            witch.Player.Exiled();
        }
    }
}