using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities;
using System.Collections;
using TownOfUs;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Events.Neutral;

public static class SoulCollectorEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!CustomRoleUtils.GetActiveRolesOfType<SoulCollectorJKRole>().HasAny())
        {
            return;
        }

        if (!OptionGroupSingleton<SoulCollectorJKOptions>.Instance.SoulCollectorArrows)
        {
            return;
        }

        Coroutines.Start(CoCreateSoulCollectorArrow(@event.Target));
    }

    public static IEnumerator CoCreateSoulCollectorArrow(PlayerControl target)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<SoulCollectorJKOptions>.Instance.SoulCollectorArrowDelay.Value);

        var deadBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == target.PlayerId);

        if (deadBody == null)
        {
            yield break;
        }

        foreach (var soulCollector in CustomRoleUtils.GetActiveRolesOfType<SoulCollectorJKRole>().Select(x => x.Player))
        {
            if (soulCollector.AmOwner)
            {
                soulCollector.AddModifier<SoulCollectorArrowModifier>(deadBody, Colors.SoulCollector);
            }
        }
    }
}