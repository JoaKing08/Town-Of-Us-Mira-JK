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
            if (!modifier.Player.HasModifier<InvulnerabilityModifier>() && !modifier.Player.HasModifier<BaseShieldModifier>() && !modifier.Player.HasModifier<FirstDeadShield>())
            {
                DeathHandlerModifier.UpdateDeathHandlerImmediate(modifier.Player, TouLocale.Get("DiedToAmmit"),
                    DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                    lockInfo: DeathHandlerOverride.SetTrue);

                modifier.Player.Exiled();
            }
            modifier.Player.RemoveModifier<AmmitDevouredModifier>();
        }
    }
}