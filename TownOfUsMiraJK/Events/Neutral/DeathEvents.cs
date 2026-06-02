using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfUsMiraJK.Modules.Components;

namespace TownOfUs.Events.Modifiers;

public static class DeathEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (ArmageddonSabotageSystem.Instance.Stage != ArmageddonStage.None)
        {
            ArmageddonSabotageSystem.RoundStart = true;
        }
    }
}