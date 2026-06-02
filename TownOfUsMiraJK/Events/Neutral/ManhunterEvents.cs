using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Events.Neutral;

public static class ManhunterEvents
{
    [RegisterEvent]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (@event.DeathReason == DeathReason.Kill)
        {
            return;
        }
        foreach (var manhunter in CustomRoleUtils.GetActiveRolesOfType<ManhunterRole>())
        {
            manhunter.CheckTargetDeath(@event.Player);
        }
    }
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        foreach (var manhunter in CustomRoleUtils.GetActiveRolesOfType<ManhunterRole>())
        {
            manhunter.CheckTargetDeath(@event.Target, @event.Source);
        }
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;

        if (exiled != null)
        {
            CustomRoleUtils.GetActiveRolesOfType<ManhunterRole>().Do(x => x.CheckTargetDeath(exiled));
        }

        var manhunter = CustomRoleUtils.GetActiveRolesOfType<ManhunterRole>().FirstOrDefault();
        if (manhunter != null && manhunter.TargetsDead && !manhunter.Player.HasDied())
        {
            if (manhunter.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRoleManhunterVictoryMessageSelf").Replace("<role>", $"{Colors.Manhunter.ToTextColor()}{manhunter.RoleName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Manhunter.LoadAsset());

                notif1.AdjustNotification();
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRoleManhunterVictoryMessage").Replace("<player>", manhunter.Player.Data.PlayerName).Replace("<role>", $"{Colors.Manhunter.ToTextColor()}{manhunter.RoleName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Manhunter.LoadAsset());

                notif1.AdjustNotification();
            }
            DeathHandlerModifier.UpdateDeathHandlerImmediate(manhunter.Player, TouLocale.Get("DiedToWinning"),
                DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                lockInfo: DeathHandlerOverride.SetTrue);

            manhunter.Player.Exiled();
        }
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        var manhunter = CustomRoleUtils.GetActiveRolesOfType<ManhunterRole>().FirstOrDefault();
        if (manhunter != null && manhunter.TargetsDead && !manhunter.Player.HasDied())
        {
            if (manhunter.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRoleManhunterVictoryMessageSelf").Replace("<role>", $"{Colors.Manhunter.ToTextColor()}{manhunter.RoleName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Manhunter.LoadAsset());

                notif1.AdjustNotification();
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRoleManhunterVictoryMessage").Replace("<player>", manhunter.Player.Data.PlayerName).Replace("<role>", $"{Colors.Manhunter.ToTextColor()}{manhunter.RoleName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Manhunter.LoadAsset());

                notif1.AdjustNotification();
            }
            DeathHandlerModifier.UpdateDeathHandlerImmediate(manhunter.Player, TouLocale.Get("DiedToWinning"),
                DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                lockInfo: DeathHandlerOverride.SetTrue);

            manhunter.Player.Exiled();
        }
    }
}