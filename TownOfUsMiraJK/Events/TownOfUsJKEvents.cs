using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfUs.Modules;

namespace TownOfUsMiraJK.Events;

public static class TownOfUsJKEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent murderEvent)
    {
        CoronerData.AddMurder(murderEvent.Source, murderEvent.Target);
    }
}