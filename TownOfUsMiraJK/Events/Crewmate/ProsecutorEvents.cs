using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Crewmate;

namespace TownOfUsMiraJK.Events.Crewmate;

public static class ProsecutorEvents
{

    [RegisterEvent(399)]
    public static void WrapUpEvent(EjectionEvent @event)
    {
        if (!OptionGroupSingleton<ProsecutorJKOptions>.Instance.Reveal)
        {
            return;
        }

        var player = @event.ExileController.initData.networkedPlayer?.Object;

        foreach (var pros in CustomRoleUtils.GetActiveRolesOfType<ProsecutorRole>())
        {
            var hasProsecuted = pros.HasProsecuted;

            if (player == null)
            {
                continue;
            }

            if (hasProsecuted)
            {
                pros.Player.AddModifier<ProsecutorRevealModifier>(pros as RoleBehaviour);
            }
        }
    }
}