using HarmonyLib;
using Hazel;
using MiraAPI.GameOptions;
using TownOfUs.Modules.Components;
using TownOfUs.Options.Roles.Impostor;
using TownOfUsMiraJK.Modules.Components;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUs.Patches.Roles;

[HarmonyPatch]
public static class DeathSabotagePatches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
    [HarmonyPatch(typeof(PolusShipStatus), nameof(PolusShipStatus.OnEnable))]
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    [HarmonyPatch(typeof(FungleShipStatus), nameof(FungleShipStatus.OnEnable))]
    [HarmonyPostfix]
    public static void AddCustomSabotageSystems(ShipStatus __instance)
    {
        if (!__instance.Systems.TryGetValue((SystemTypes)ArmageddonSabotageSystem.SabotageId, out _))
        {
            var hexBombSabo = new ArmageddonSabotageSystem(OptionGroupSingleton<ReaperJKOptions>.Instance.ArmageddonTimer);
            __instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>().specials
                .Add(hexBombSabo.Cast<IActivatable>());
            __instance.Systems.Add((SystemTypes)ArmageddonSabotageSystem.SabotageId, hexBombSabo.Cast<ISystemType>());
        }
    }

    [HarmonyPatch(typeof(SabotageSystemType), nameof(SabotageSystemType.UpdateSystem))]
    [HarmonyPostfix]
    public static void UpdateSystemPatch([HarmonyArgument(0)] PlayerControl player, [HarmonyArgument(1)] MessageReader reader)
    {
        var amount = reader.Buffer[reader.readHead - 1];

        if (AmongUsClient.Instance.AmHost && MeetingHud.Instance == null && ExileController.Instance == null &&
            amount == ArmageddonSabotageSystem.SabotageId)
        {
            ShipStatus.Instance.UpdateSystem((SystemTypes)ArmageddonSabotageSystem.SabotageId, player, 1);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AddSystemTask))]
    [HarmonyPrefix]
    public static bool AddSaboTask(PlayerControl __instance, ref SystemTypes system, ref PlayerTask __result)
    {
        if (!__instance.AmOwner) return true;

        if (system == (SystemTypes)ArmageddonSabotageSystem.SabotageId)
        {
            var task = new GameObject("ArmageddonTask").AddComponent<ArmageddonSabotageTask>();
            task.gameObject.transform.SetParent(__instance.gameObject.transform);
            task.Id = 255U;
            task.Owner = __instance;
            task.Initialize();
            __instance.myTasks.Add(task);
            __result = task;
            return false;
        }
        return true;
    }
}