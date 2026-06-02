using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Roles;
using TownOfUsMiraJK.Roles.Neutral;

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