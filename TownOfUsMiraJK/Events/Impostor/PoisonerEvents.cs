using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Buttons.Impostor;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUsMiraJK.Events.Impostor;

public static class PoisonerEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;
        if (source.IsImpostor() && source.AmOwner && source != target && !MeetingHud.Instance && source.IsRole<PoisonerRole>())
        {
            var poisonButton = CustomButtonSingleton<PoisonerPoisonButton>.Instance;
            poisonButton.ResetCooldownAndOrEffect();
        }
    }

    [RegisterEvent(-1000)]
    public static void ReportBodyEventHandler(ReportBodyEvent @event)
    {
        foreach (var modifier in ModifierUtils.GetActiveModifiers<PoisonerPoisonModifier>())
        {
            modifier.ModifierComponent?.RemoveModifier(modifier);
        }
    }
}