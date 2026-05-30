using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs.Modifiers.Game.Impostor;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers.Game.Impostor;
using TownOfUsMiraJK.Options.Modifiers.Impostor;

namespace TownOfUsMiraJK.Events.Modifiers;

public static class OutcastEvents
{
    [RegisterEvent(1)]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;

        // Scavenger already handles it's own Kill timer
        if (!source.HasModifier<OutcastModifier>() || source.IsRole<ScavengerRole>())
        {
            return;
        }

        source.SetKillTimer(source.GetKillCooldown());
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!PlayerControl.LocalPlayer.HasModifier<OutcastModifier>() ||
            PlayerControl.LocalPlayer.IsRole<ScavengerRole>())
        {
            return;
        }

        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());
    }
    [HarmonyPatch(typeof(UnderdogModifier),nameof(UnderdogModifier.GetKillCooldown))]
    public static class GetKillCooldownPatch
    {
        public static void Postfix(PlayerControl player, ref float __result)
        {
            if (!player.HasModifier<OutcastModifier>())
            {
                return;
            }
            __result -= OptionGroupSingleton<OutcastOptions>.Instance.KillCooldownDecrease;
        }
    }
}