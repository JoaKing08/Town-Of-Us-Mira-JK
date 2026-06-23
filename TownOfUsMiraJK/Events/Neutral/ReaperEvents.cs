using AmongUs.GameOptions;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities;
using System.Collections;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Events.Neutral;

public static class ReaperEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!CustomRoleUtils.GetActiveRolesOfType<ReaperRole>().HasAny())
        {
            return;
        }

        if (!OptionGroupSingleton<ReaperOptions>.Instance.ReaperArrows)
        {
            return;
        }

        Coroutines.Start(CoCreateReaperArrow(@event.Target));
    }

    public static IEnumerator CoCreateReaperArrow(PlayerControl target)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<ReaperOptions>.Instance.ReaperArrowDelay.Value);

        var deadBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == target.PlayerId);

        if (deadBody == null)
        {
            yield break;
        }

        foreach (var soulCollector in CustomRoleUtils.GetActiveRolesOfType<ReaperRole>().Select(x => x.Player))
        {
            if (soulCollector.AmOwner)
            {
                soulCollector.AddModifier<ReaperArrowModifier>(deadBody, TownOfUsMiraJKColors.Reaper);
            }
        }
    }
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        ReaperRole.ReapedBodies.Clear();
    }
}