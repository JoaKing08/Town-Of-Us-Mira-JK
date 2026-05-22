using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modifiers.Game.Crewmate;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules;
using TownOfUs.Options.Maps;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;
using UnityEngine;

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