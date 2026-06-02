using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class UndercoverNoVentMovePatch
{
    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    [HarmonyPrefix]
    public static bool UndercoverEnterVent()
    {
        if (!PlayerControl.LocalPlayer)
        {
            return true;
        }

        if (!PlayerControl.LocalPlayer.Data)
        {
            return true;
        }

        if (PlayerControl.LocalPlayer.Data.Role is UndercoverRole && !OptionGroupSingleton<UndercoverOptions>.Instance.CanMoveInVent)
        {
            return false;
        }

        return true;
    }
}