using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs;
using TownOfUs.Buttons.BaseFreeplay;
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