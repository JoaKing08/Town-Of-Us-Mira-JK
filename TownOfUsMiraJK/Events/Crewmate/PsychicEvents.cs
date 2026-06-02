using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUs.Events.Crewmate;

public static class PsychicEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        PsychicRole.RoundStart = DateTime.UtcNow;
        if (PlayerControl.LocalPlayer.HasDied())
        {
            foreach (var modifier in ModifierUtils.GetActiveModifiers<PsychicColoredModifier>())
            {
                modifier.ModifierComponent?.RemoveModifier(modifier);
            }
        }
    }
}