using HarmonyLib;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Patches;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class UndeadPatches
{
    [HarmonyPatch(typeof(LogicGameFlowPatches), nameof(LogicGameFlowPatches.CheckEndGameViaHexBomb))]
    [HarmonyPrefix]
    public static bool Prefix(ref bool __result)
    {
        if (CustomRoleUtils.GetActiveRolesOfType<DeathRole>().FirstOrDefault()?.Player.HasModifier<NecromancerUndeadModifier>() != false)
        {
            return true;
        }
        __result = false;
        return false;
    }
}