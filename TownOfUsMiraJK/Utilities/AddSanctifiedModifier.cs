using HarmonyLib;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Utilities
{
    [HarmonyPatch]
    public static class AddSanctifierModifier
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]
        public static void Postfix(PlayerControl __instance)
        {
            if (LobbyBehaviour.Instance)
            {
                return;
            }
            if (!CustomRoleUtils.GetActiveRolesOfType<SanctifierRole>().Any())
            {
                return;
            }
            if (!PlayerControl.LocalPlayer.IsHost())
            {
                return;
            }
            if (SanctifierCircle.SanctifierCircles.Count != 0 && !PlayerControl.LocalPlayer.HasDied() && SanctifierCircle.IsInCircle(PlayerControl.LocalPlayer.transform))
            {
                if (!PlayerControl.LocalPlayer.HasModifier<SanctifiedModifier>())
                {
                    PlayerControl.LocalPlayer.RpcAddModifier<SanctifiedModifier>();
                }
            }
            else
            {
                if (PlayerControl.LocalPlayer.HasModifier<SanctifiedModifier>())
                {
                    PlayerControl.LocalPlayer.RpcRemoveModifier<SanctifiedModifier>();
                }
            }
        }
    }
}
