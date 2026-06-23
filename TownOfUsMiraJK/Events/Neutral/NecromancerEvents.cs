using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using Reactor.Utilities;
using System.Collections;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Events.Neutral;

public static class NecromancerEvents
{
    [RegisterEvent(400)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (@event.Player.IsRole<NecromancerRole>())
        {
            foreach (var undead in ModifierUtils.GetPlayersWithModifier<NecromancerUndeadModifier>(x => !x.Player.HasDied()))
            {
                switch (@event.DeathReason)
                {
                    case DeathReason.Exile:
                        DeathHandlerModifier.UpdateDeathHandlerImmediate(undead, TouLocale.Get("DiedToNecromancer"),
                            DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                            lockInfo: DeathHandlerOverride.SetTrue);
                        undead.Exiled();
                        break;
                    case DeathReason.Kill:
                        var showAnim = MeetingHud.Instance == null && ExileController.Instance == null;
                        var murderResultFlags2 = MurderResultFlags.DecisionByHost | MurderResultFlags.Succeeded;

                        DeathHandlerModifier.UpdateDeathHandlerImmediate(undead, TouLocale.Get("DiedToNecromancer"),
                            DeathEventHandlers.CurrentRound,
                            !MeetingHud.Instance && !ExileController.Instance
                                ? DeathHandlerOverride.SetTrue
                                : DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
                        undead.CustomMurder(
                            undead,
                            murderResultFlags2,
                            false,
                            showAnim,
                            false,
                            showAnim,
                            false);
                        break;
                    default:
                        if (MeetingHud.Instance || ExileController.Instance)
                        {
                            goto case DeathReason.Exile;
                        }
                        else
                        {
                            goto case DeathReason.Kill;
                        }
                }
            }
        }
    }

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
                necromancer.AddModifier<NecromancerArrowModifier>(deadBody, TownOfUsMiraJKColors.Necromancer);
            }
        }
    }
}