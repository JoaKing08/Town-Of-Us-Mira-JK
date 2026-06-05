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
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Events.Crewmate;

public static class TavernKeeperEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        if (!OptionGroupSingleton<TavernKeeperOptions>.Instance.ResetEveryRound)
        {
            return;
        }

        if (!PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<TavernKeeperRole>()))
        {
            return;
        }

        CustomButtonSingleton<TavernKeeperDrinkButton>.Instance.SetUses((int)OptionGroupSingleton<TavernKeeperOptions>.Instance.MaxDrinks);
    }
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var source = PlayerControl.LocalPlayer;
        var button = @event.Button;

        if (button == null || !button.CanClick())
        {
            return;
        }

        CheckForDrunk(@event, source);
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

        CheckForDrunk(@event, source);
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;

        CheckForDrunk(@event, source);
    }

    private static void CheckForDrunk(MiraCancelableEvent miraEvent, PlayerControl source)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (!source.HasModifier<TavernKeeperDrunkModifier>())
        {
            return;
        }

        if (source.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleTavernKeeperDrinkNotif").Replace("<role>",
                MiscUtils.GetHyperlinkText(MiscUtils.PlayerById(source.GetModifier<TavernKeeperDrunkModifier>()!.TavernKeeperId).Data.Role)),
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.TavernKeeper.LoadAsset());

            notif1.AdjustNotification();
        }

        miraEvent.Cancel();
    }
}