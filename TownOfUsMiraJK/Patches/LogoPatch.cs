using HarmonyLib;
using MiraAPI.Utilities.Assets;
using TownOfUsMiraJK.Assets;
using TownOfUs.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class LogoPatch
{
    [HarmonyPatch(typeof(TouAssets), nameof(TouAssets.Banner), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool Prefix(ref LoadableAsset<Sprite> __result)
    {
        __result = ToUJKAssets.Banner;
        return false;
    }
}