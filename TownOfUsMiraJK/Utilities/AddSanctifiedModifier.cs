using HarmonyLib;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Utilities;

namespace TownOfUsMiraJK.Utilities
{
    [HarmonyPatch]
    public static class AddSanctifierModifier
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.AmOwner)
            {
                return;
            }
            if (!PlayerControl.LocalPlayer.IsHost() && !TutorialManager.Instance)
            {
                return;
            }
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected))
            {
                if (!player.HasDied() && SanctifierCircle.IsInCircle(player.transform))
                {
                    if (!player.HasModifier<SanctifiedModifier>())
                    {
                        player.RpcAddModifier<SanctifiedModifier>();
                    }
                }
                else
                {
                    if (player.HasModifier<SanctifiedModifier>())
                    {
                        player.RpcRemoveModifier<SanctifiedModifier>();
                    }
                }
            }
        }
    }
}
