using HarmonyLib;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.MedSpirit;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles.Crewmate;
using TownOfUsMiraJK.Modules.Components;
using UnityEngine.ProBuilder;

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