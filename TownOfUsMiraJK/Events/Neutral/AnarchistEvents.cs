using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Events.Neutral;

public static class AnarchistEvents
{
    [RegisterEvent(0)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (@event.DeathReason is DeathReason.Exile)
        {
            var victim = @event.Player;
            if (!victim.IsCrewmate() || victim.HasModifier<AllianceGameModifier>())
            {
                return;
            }

            foreach (var ana in CustomRoleUtils.GetActiveRolesOfType<AnarchistRole>())
            {
                if (!ana.Misejected.Any(x => x.PlayerId == victim.PlayerId))
                {
                    ana.Misejected.Add(victim);
                }
            }
        }
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        foreach (var anarchist in CustomRoleUtils.GetActiveRolesOfType<AnarchistRole>())
        {
            if (!anarchist.AboutToWin)
            {
                anarchist.Voters.Clear();
            }
        }

        if (@event.TriggeredByIntro)
        {
            return;
        }

        var winOption = OptionGroupSingleton<AnarchistOptions>.Instance.AnarchistWin;
        
        var ana = CustomRoleUtils.GetActiveRolesOfType<AnarchistRole>()
            .FirstOrDefault(x => x.AboutToWin && !x.Player.HasDied());

        if (winOption is AnaWinOptions.EndsGame)
        {
            return;
        }

        if (ana != null)
        {
            if (ana.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TouLocale.GetParsed("TouJKRoleAnarchistWonSelf").Replace("<role>", $"{Colors.Anarchist.ToTextColor()}{ana.RoleName}</color>")}</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Anarchist.LoadAsset());

                notif1.AdjustNotification();

                PlayerControl.LocalPlayer.RpcPlayerExile();

                if (winOption is AnaWinOptions.Assaults)
                {
                    CustomButtonSingleton<AnarchistAssaultButton>.Instance.SetActive(true, ana);
                    DeathHandlerModifier.RpcUpdateLocalDeathHandler(PlayerControl.LocalPlayer,
                        "DiedToWinning", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetTrue,
                        lockInfo: DeathHandlerOverride.SetTrue);
                    var notif2 = Helpers.CreateAndShowNotification(
                        $"<b>{TouLocale.GetParsed("TouJKRoleAnarchistAssaultFeedback")}</b>",
                        Color.white, new Vector3(0f, 0.85f, -20f));
                    notif2.AdjustNotification();
                }
                else
                {
                    DeathHandlerModifier.RpcUpdateLocalDeathHandler(PlayerControl.LocalPlayer,
                        "DiedToWinning", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                        lockInfo: DeathHandlerOverride.SetTrue);
                }
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TouLocale.GetParsed("TouJKRoleAnarchistWonOther").Replace("<player>", ana.Player.Data.PlayerName).Replace("<role>", $"{Colors.Anarchist.ToTextColor()}{ana.RoleName}</color>")}</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Anarchist.LoadAsset());

                notif1.AdjustNotification();
            }
        }
    }

    [RegisterEvent]
    public static void HandleVoteEventHandler(HandleVoteEvent @event)
    {
        var votingPlayer = @event.Player;
        var suspectPlayer = @event.TargetPlayerInfo;

        if (suspectPlayer == null)
        {
            return;
        }

        foreach (var ana in CustomRoleUtils.GetActiveRolesOfType<AnarchistRole>())
        {
            if (!ana.Player.HasDied())
            {
                ana.Voters.Add((suspectPlayer.PlayerId, votingPlayer.PlayerId));
            }
        }
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;

        if (exiled == null || !exiled.IsCrewmate() || exiled.HasModifier<AllianceGameModifier>())
        {
            return;
        }

        foreach (var ana in CustomRoleUtils.GetActiveRolesOfType<AnarchistRole>())
        {
            if (ana.Misejects + 1 >= OptionGroupSingleton<AnarchistOptions>.Instance.MisejectsCount)
            {
                ana.AboutToWin = true;
            }
            if (!PlayerControl.LocalPlayer.IsHost() && !ana.Misejected.Any(x => x.PlayerId == exiled.PlayerId))
            {
                ana.Misejected.Add(exiled);
            }

            var winOption = OptionGroupSingleton<AnarchistOptions>.Instance.AnarchistWin;

            if (ana.Player.AmOwner && winOption is AnaWinOptions.Assaults)
            {
                var allVoters = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => ana.Voters.Contains((exiled.PlayerId, x.PlayerId)) && !x.AmOwner);

                if (!allVoters.HasAny())
                {
                    return;
                }

                foreach (var player in allVoters)
                {
                    player.AddModifier<MisfortuneTargetModifier>();
                }

                CustomButtonSingleton<AnarchistAssaultButton>.Instance.Show = true;
            }
        }
    }
}