using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Buttons.Secret;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Secret;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Secret;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Events.Secret;

public static class AmmitEvents
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

        if (!PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<AmmitRole>()))
        {
            return;
        }

        if (OptionGroupSingleton<AmmitOptions>.Instance.MaxDevoured > 0f)
        {
            CustomButtonSingleton<AmmitDevourButton>.Instance.SetUses((int)OptionGroupSingleton<AmmitOptions>.Instance.MaxDevoured);
        }
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

        CheckForDrunk(@event, source, true);
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

        CheckForDrunk(@event, source, true);
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;

        CheckForDrunk(@event, source);
    }

    private static void CheckForDrunk(MiraCancelableEvent miraEvent, PlayerControl source, bool isLocal = false)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (!source.AmOwner)
        {
            return;
        }

        if (!source.HasModifier<TavernKeeperDrunkModifier>())
        {
            return;
        }

        miraEvent.Cancel();

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed("TouJKRoleTavernKeeperDrunkNotif").Replace("<role>",
            MiscUtils.GetHyperlinkText(MiscUtils.PlayerById(source.GetModifier<TavernKeeperDrunkModifier>()!.TavernKeeperId).Data.Role)),
            Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.TavernKeeper.LoadAsset());

        notif1.AdjustNotification();
    }
}