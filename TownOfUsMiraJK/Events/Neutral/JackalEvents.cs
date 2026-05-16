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
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Collections;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Events;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Events.Neutral;

public static class GodfatherEvents
{
    [RegisterEvent(400)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (@event.DeathReason == DeathReason.Exile)
        {
            if (!ModifierUtils.GetActiveModifiers<JackalRecruitModifier>(x => !x.Player.HasDied() && !x.Player.IsCrewmate()).Any())
            {
                var jackal = CustomRoleUtils.GetActiveRolesOfType<JackalRole>().FirstOrDefault(x => !x.UnlockedKill);
                if (jackal != null)
                {
                    jackal.UnlockedKill = true;
                    if (jackal.Player.AmOwner)
                    {
                        var notif1 = Helpers.CreateAndShowNotification(
                            TouLocale.GetParsed("TouJKRoleJackalKillUnlockNotif"),
                            Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Jackal.LoadAsset());

                        notif1.AdjustNotification();
                    }
                }
            }
        }
        if (@event.Player == null)
        {
            return;
        }

        if (!@event.Player.TryGetModifier<JackalRecruitModifier>(out var recruitMod))
        {
            return;
        }

        if (PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<JackalRole>()))
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleJackalRecDeadNotif").Replace("<player>", $"{Colors.Jackal.ToHtmlStringRGBA()}{@event.Player.Data.PlayerName}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Jackal.LoadAsset());

            notif1.AdjustNotification();
        }

        if (!OptionGroupSingleton<JackalOptions>.Instance.RecruitsDieTogether || recruitMod.OtherRecruit == null
            || recruitMod.OtherRecruit.Player.HasDied() || recruitMod.OtherRecruit.Player.HasModifier<InvulnerabilityModifier>())
        {
            return;
        }

        switch (@event.DeathReason)
        {
            case DeathReason.Exile:
                DeathHandlerModifier.UpdateDeathHandlerImmediate(recruitMod.OtherRecruit.Player, TouLocale.Get("DiedToBrotherhood"),
                    DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                    lockInfo: DeathHandlerOverride.SetTrue);
                recruitMod.OtherRecruit.Player.Exiled();
                break;
            case DeathReason.Kill:
                var showAnim = MeetingHud.Instance == null && ExileController.Instance == null;
                var murderResultFlags2 = MurderResultFlags.DecisionByHost | MurderResultFlags.Succeeded;

                DeathHandlerModifier.UpdateDeathHandlerImmediate(recruitMod.OtherRecruit.Player, TouLocale.Get("DiedToBrotherhood"),
                    DeathEventHandlers.CurrentRound,
                    (!MeetingHud.Instance && !ExileController.Instance)
                        ? DeathHandlerOverride.SetTrue
                        : DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
                recruitMod.OtherRecruit.Player.CustomMurder(
                    recruitMod.OtherRecruit.Player,
                    murderResultFlags2,
                    false,
                    showAnim,
                    false,
                    showAnim,
                    false);
                break;
        }
    }
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        if (!ModifierUtils.GetActiveModifiers<JackalRecruitModifier>(x => !x.Player.HasDied() && !x.Player.IsCrewmate()).Any())
        {
            var jackal = CustomRoleUtils.GetActiveRolesOfType<JackalRole>().FirstOrDefault(x => !x.UnlockedKill);
            if (jackal != null)
            {
                jackal.UnlockedKill = true;
                if (jackal.Player.AmOwner)
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRoleJackalKillUnlockNotif"),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Jackal.LoadAsset());

                    notif1.AdjustNotification();
                }
            }
        }
    }
}