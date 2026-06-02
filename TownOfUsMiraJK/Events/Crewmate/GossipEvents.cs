using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Modifiers.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUs.Events.Crewmate;

public static class GossipEvents
{
    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        CustomRoleUtils.GetActiveRolesOfType<GossipRole>().Do(x => x.ReportOnChat());
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        if (!AmongUsClient.Instance.AmHost)
        {
            return;
        }

        ModifierUtils.GetPlayersWithModifier<GossipChatModifier>()
            .Do(x => x.RpcRemoveModifier<GossipChatModifier>());
    }
}