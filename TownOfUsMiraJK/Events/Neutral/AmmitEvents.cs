using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;

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

        foreach (var player in ModifierUtils.GetPlayersWithModifier<AmmitDevouredModifier>())
        {
            player.RemoveModifier<AmmitDevouredModifier>();
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
        foreach (var player in ModifierUtils.GetPlayersWithModifier<AmmitDevouredModifier>())
        {
            if (!player.HasModifier<InvulnerabilityModifier>() && !player.HasModifier<BaseShieldModifier>() && !player.HasModifier<FirstDeadShield>())
            {
                DeathHandlerModifier.UpdateDeathHandlerImmediate(player, TouLocale.Get("DiedToAmmit"),
                    DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                    lockInfo: DeathHandlerOverride.SetTrue);

                player.Exiled();
            }
            player.RemoveModifier<AmmitDevouredModifier>();
        }
    }
}