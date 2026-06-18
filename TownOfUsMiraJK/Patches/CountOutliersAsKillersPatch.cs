using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class CountOutliersAsKillersPatch
{
    [HarmonyPatch(typeof(MiscUtils), nameof(MiscUtils.KillersAliveCount), MethodType.Getter)]
    [HarmonyPostfix]
    public static void AddOutliers(ref int __result)
    {
        __result += CustomRoleUtils.GetActiveRoles().Count(x => ((x.IsApocalypse() && x.GetRoleAlignment() != TownOfUs.Roles.RoleAlignment.NeutralKilling) || x is NecromancerRole) && x.Player?.HasDied() == false);
    }
}