using HarmonyLib;
using MiraAPI.Modifiers;
using TownOfUs;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;

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
    }
}