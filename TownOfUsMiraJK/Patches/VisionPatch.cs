using HarmonyLib;
using MiraAPI.Modifiers;
using TownOfUs;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Modifiers.Alchemist;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
[HarmonyAfter(TownOfUsPlugin.Id)]
public static class VisionPatch
{
    public static void Postfix(ShipStatus __instance, NetworkedPlayerInfo player, ref float __result)
    {
        if (MiscUtils.CurrentGamemode() is TouGamemode.HideAndSeek || player == null || player.IsDead)
        {
            return;
        }
        if (player.Object?.TryGetModifier<ShadowDarknessModifier>(out var modifier) == true)
        {
            __result *= modifier.VisionPerc;
        }
        if (player.Object?.TryGetModifier<BlindnessPotionModifier>(out var modifier1) == true)
        {
            __result *= modifier1.VisionPerc;
        }
        if (player.Object?.TryGetModifier<PerceptionPotionModifier>(out var modifier2) == true)
        {
            __result *= modifier2.VisionPerc;
        }
        if (player.Object?.TryGetModifier<SleepPotionModifier>(out var modifier3) == true)
        {
            __result *= modifier3.VisionPerc;
        }
    }
}