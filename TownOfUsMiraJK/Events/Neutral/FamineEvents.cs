using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUs.Events.Modifiers;

public static class FamineEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        var fam = CustomRoleUtils.GetActiveRolesOfType<FamineRole>().FirstOrDefault();
        if (fam == null)
        {
            return;
        }
        fam.DoPassiveStarve();
    }
}