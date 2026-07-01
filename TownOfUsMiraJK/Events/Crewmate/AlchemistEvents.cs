using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Modifiers.Alchemist;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Events.Crewmate;

public static class AlchemistEvents
{
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var source = PlayerControl.LocalPlayer;
        var button = @event.Button;

        if (button == null || !button.CanClick())
        {
            return;
        }

        CheckForAlcohol(@event, source);
        if (button is CustomActionButton<PlayerControl> actionButton && actionButton?.Target != null && button is IKillButton)
        {
            CheckForShield(@event, source, actionButton.Target);
        }
    }
    [RegisterEvent]
    public static void VanillaButtonClickEventHandler(VanillaButtonClickEvent @event)
    {
        var source = PlayerControl.LocalPlayer;
        var button = @event.Button;

        if (button == null)
        {
            return;
        }

        CheckForAlcohol(@event, source);
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;
        if (source == null || target == source)
        {
            return;
        }
        CheckForAlcohol(@event, source);
        CheckForShield(@event, source, target);
    }

    private static void CheckForAlcohol(MiraCancelableEvent miraEvent, PlayerControl source)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (!source.HasModifier<AlcoholModifier>())
        {
            return;
        }

        if (source.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleTavernKeeperDrinkNotif").Replace("<role>",
                $"{TownOfUsMiraJKColors.Alchemist.ToTextColor()}{TouLocale.Get("TouJKRoleAlchemist")}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Alchemist.LoadAsset());

            notif1.AdjustNotification();
        }

        miraEvent.Cancel();
    }

    private static void CheckForShield(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (!target.HasModifier<ShieldPotionModifier>() ||
            target.PlayerId == source.PlayerId ||
            (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield))
        {
            return;
        }

        @event.Cancel();

        if (@event is MiraButtonClickEvent buttonClick)
        {
            var button = buttonClick.Button;
            if (button != null)
            {
                button.Timer = OptionGroupSingleton<GameMechanicOptions>.Instance.TempSaveCdReset;
            }
        }

        if (@event is BeforeMurderEvent && source.IsImpostor())
        {
            source.SetKillTimer(OptionGroupSingleton<GameMechanicOptions>.Instance.TempSaveCdReset);
        }

        MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Error, $"{target.Data.PlayerName} has a alchemist shield, stopping an attack from {source.Data.PlayerName}!");

        var alchemist = target.GetModifier<ShieldPotionModifier>()?.Alchemist.GetRole<AlchemistRole>();

        if (alchemist != null && (TutorialManager.InstanceExists || source.AmOwner))
        {
            AlchemistRole.RpcAlchemistNotify(alchemist.Player, source, target);
        }
    }
}