using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities;
using System.Collections;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Events.Neutral;

public static class ReaperEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!CustomRoleUtils.GetActiveRolesOfType<ReaperJKRole>().HasAny())
        {
            return;
        }

        if (!OptionGroupSingleton<ReaperJKOptions>.Instance.ReaperArrows)
        {
            return;
        }

        Coroutines.Start(CoCreateReaperArrow(@event.Target));
    }

    public static IEnumerator CoCreateReaperArrow(PlayerControl target)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<ReaperJKOptions>.Instance.ReaperArrowDelay.Value);

        var deadBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == target.PlayerId);

        if (deadBody == null)
        {
            yield break;
        }

        foreach (var soulCollector in CustomRoleUtils.GetActiveRolesOfType<ReaperJKRole>().Select(x => x.Player))
        {
            if (soulCollector.AmOwner)
            {
                soulCollector.AddModifier<ReaperArrowModifier>(deadBody, TownOfUsMiraJKColors.Reaper);
            }
        }
    }
}