using HarmonyLib;
using MiraAPI.Utilities;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class VisorEffectsPatches
{
    [HarmonyPatch(typeof(EclipsalBlindModifier), nameof(EclipsalBlindModifier.FixedUpdate))]
    [HarmonyPatch(typeof(GrenadierFlashModifier), nameof(GrenadierFlashModifier.FixedUpdate))]
    [HarmonyPostfix]
    public static void EclipsalBlindModifierFixedUpdatePostfix(EclipsalBlindModifier __instance)
    {
        foreach (var undercover in Helpers.GetAlivePlayers().Where(x => x.IsRole<UndercoverRole>()))
        {
            undercover.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, Palette.VisorColor);
        }
    }
}