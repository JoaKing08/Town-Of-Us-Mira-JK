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
using TownOfUs.Events;
using TownOfUs.Modifiers;
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
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Events.Neutral;

public static class AmmitEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
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

    [RegisterEvent(-1000)]
    public static void ReportBodyEventHandler(ReportBodyEvent @event)
    {
        foreach (var modifier in ModifierUtils.GetActiveModifiers<AmmitDevouredModifier>())
        {
            if (!modifier.Player.HasModifier<InvulnerabilityModifier>())
            {
                DeathHandlerModifier.UpdateDeathHandlerImmediate(modifier.Player, TouLocale.Get("DiedToAmmit"),
                    DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                    lockInfo: DeathHandlerOverride.SetTrue);

                modifier.Player.Exiled();
            }
            modifier.ModifierComponent?.RemoveModifier(modifier);
        }
    }
}