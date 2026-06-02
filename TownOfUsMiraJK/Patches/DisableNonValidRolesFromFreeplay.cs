using HarmonyLib;
using MiraAPI.Roles;
using TownOfUs.Buttons.BaseFreeplay;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch(typeof(FreeplaySetRolesButton), "IsRoleValid")]
public static class FreeplayIsRoleValidPatch
{
    public static void Postfix(RoleBehaviour role, ref bool __result)
    {
        if (role is ICustomRole { Configuration.ShowInFreeplay: false })
        {
            __result = false;
        }
    }
}