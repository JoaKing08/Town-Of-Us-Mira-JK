using Cpp2IL.Core.Utils;
using HarmonyLib;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class AutoImitatorPatches
{
    [HarmonyPatch(typeof(ImitatorRole), nameof(ImitatorRole.Abilities), MethodType.Getter)]
    [HarmonyPostfix]
    public static void ChangeResults(ImitatorRole __instance, ref List<CustomButtonWikiDescription> __result)
    {
        var neutDescription = "";
        var impDescription = "";

        var neutRoles = new List<RoleBehaviour>();
        foreach (var role in CustomRoleManager.AllRoles.Select(x => x is IUnguessable unguessable ? unguessable.AppearAs : x).Where(x => !x.IsCrewmate() && !x.IsImpostor() && x is ICrewVariant && x is not PlaguebearerRole))
        {
            if (!neutRoles.Any(x => x.Role == role.Role))
            {
                neutRoles.Add(role);
            }
        }
        neutRoles = neutRoles.OrderBy(x => x.GetRoleName()).ToList();

        var impRoles = new List<RoleBehaviour>();
        foreach (var role in CustomRoleManager.AllRoles.Select(x => x is IUnguessable unguessable ? unguessable.AppearAs : x).Where(x => x.IsImpostor() && x is ICrewVariant))
        {
            if (!impRoles.Any(x => x.Role == role.Role))
            {
                impRoles.Add(role);
            }
        }
        impRoles = impRoles.OrderBy(x => x.GetRoleName()).ToList();

        var i = 0;
        foreach (var role in neutRoles)
        {
            var variant = (role as ICrewVariant)!.CrewVariant;
            if (i >= 2)
            {
                neutDescription += $"{role.GetRoleName()} ⇨ {variant.GetRoleName()}\n";
                i = 0;
            }
            else
            {
                neutDescription += $"{role.GetRoleName()} ⇨ {variant.GetRoleName()} | ";
                i++;
            }
        }
        neutDescription = neutDescription.Remove(neutDescription.Length - (i == 0 ? 1 : 3));

        i = 0;
        foreach (var role in impRoles)
        {
            var variant = (role as ICrewVariant)!.CrewVariant;
            if (i >= 2)
            {
                impDescription += $"{role.GetRoleName()} ⇨ {variant.GetRoleName()}\n";
                i = 0;
            }
            else
            {
                impDescription += $"{role.GetRoleName()} ⇨ {variant.GetRoleName()} | ";
                i++;
            }
        }
        impDescription = impDescription.Remove(impDescription.Length - (i == 0 ? 1 : 3));

        var neutObject = __result[1];
        var impObject = __result[2];
        neutObject.description = neutDescription;
        impObject.description = impDescription;
        __result[1] = neutObject;
        __result[2] = impObject;
    }
}