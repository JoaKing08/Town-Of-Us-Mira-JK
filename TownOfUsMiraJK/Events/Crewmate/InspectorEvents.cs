using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Modifiers.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUs.Events.Crewmate;

public static class InspectorEvents
{
    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        CustomRoleUtils.GetActiveRolesOfType<InspectorRole>().Do(x => x.ReportOnInspect());
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        ModifierUtils.GetPlayersWithModifier<InspectorInspectModifier>()
            .Do(x => x.RemoveModifier<InspectorInspectModifier>());
    }
}