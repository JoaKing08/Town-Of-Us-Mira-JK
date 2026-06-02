using HarmonyLib;
using TownOfUsMiraJK.Modules.Components;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class AmongUsClientPatches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
    [HarmonyPostfix]
    public static void StartPatch(AmongUsClient __instance)
    {
        if (AmongUsClient.Instance != __instance)
        {
            Error("AmongUsClient duplicate detected.");
            return;
        }

        SystemTypeHelpers.AllTypes = SystemTypeHelpers.AllTypes.Concat([(SystemTypes)ArmageddonSabotageSystem.SabotageId]).ToArray();
    }
}