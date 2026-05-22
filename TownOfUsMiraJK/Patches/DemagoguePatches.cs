using HarmonyLib;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUs.Patches.Roles;

[HarmonyPatch]
public static class DemagoguePatches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.HandleText))]
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.HandleText))]
    [HarmonyPrefix]
    public static void TranslationControllerGetStringPostfix(ExileController __instance)
    {
        if (__instance.initData.networkedPlayer != null)
        {
            return;
        }

        var immunitizedPlayers = new List<string>();
        bool easterEgg = false;

        foreach (var dem in CustomRoleUtils.GetActiveRolesOfType<DemagogueRole>())
        {
            if (!dem.Immunitized)
                continue;

            dem.Immunitized = false;
            easterEgg |= dem.EasterEgg;
            immunitizedPlayers.Add(dem.Player.GetDefaultAppearance().PlayerName);
        }

        if (immunitizedPlayers.Count >= 1)
        {
            if (easterEgg)
            {
                __instance.completeString = $"{immunitizedPlayers[0]} has extraterritorial rights and couldn't be ejected!";
            }
            else
            {
                __instance.completeString = $"{immunitizedPlayers[0]} has immunity and couldn't be ejected!";
            }
        }
    }
    [HarmonyPatch(typeof(DeputyRole), nameof(DeputyRole.IsExempt))]
    [HarmonyPostfix]
    public static void DeputyIsExempt(PlayerVoteArea voteArea, ref bool __result)
    {
        __result |= voteArea.GetPlayer()?.IsRole<DemagogueRole>() == true;
    }
    [HarmonyPatch(typeof(JailorRole), "GenButton")]
    [HarmonyPrefix]
    public static bool JailorCanExecute(PlayerVoteArea voteArea)
    {
        return voteArea.GetPlayer()?.IsRole<DemagogueRole>() != true;
    }
}