using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using System.Collections;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Networking;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Events.Neutral;

public static class NecromancerEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!CustomRoleUtils.GetActiveRolesOfType<NecromancerRole>().HasAny())
        {
            return;
        }

        if (!OptionGroupSingleton<NecromancerOptions>.Instance.NecromancerArrows)
        {
            return;
        }

        Coroutines.Start(CoCreateNecromancerArrow(@event.Target));

        if (PlayerControl.LocalPlayer.HasModifier<NecromancerUndeadModifier>() && !PlayerControl.LocalPlayer.HasDied() && !Helpers.GetAlivePlayers().Any(x => x.Is((RoleTypes)RoleId.Get<NecromancerRole>())))
        {
            PlayerControl.LocalPlayer.RpcSpecialMurder(PlayerControl.LocalPlayer, true, true, resetKillTimer: false, teleportMurderer: false, causeOfDeath: "Necromancer");
        }
    }

    [RegisterEvent]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (PlayerControl.LocalPlayer.HasModifier<NecromancerUndeadModifier>() && !PlayerControl.LocalPlayer.HasDied() && !Helpers.GetAlivePlayers().Any(x => x.Is((RoleTypes)RoleId.Get<NecromancerRole>())))
        {
            PlayerControl.LocalPlayer.RpcSpecialMurder(PlayerControl.LocalPlayer, true, true, resetKillTimer: false, teleportMurderer: false, causeOfDeath: "Necromancer");
        }
    }

    public static IEnumerator CoCreateNecromancerArrow(PlayerControl target)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<NecromancerOptions>.Instance.NecromancerArrowDelay.Value);

        var deadBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == target.PlayerId);

        if (deadBody == null)
        {
            yield break;
        }

        foreach (var necromancer in CustomRoleUtils.GetActiveRolesOfType<NecromancerRole>().Select(x => x.Player))
        {
            if (necromancer.AmOwner)
            {
                necromancer.AddModifier<NecromancerArrowModifier>(deadBody, Colors.Necromancer);
            }
        }
    }
}