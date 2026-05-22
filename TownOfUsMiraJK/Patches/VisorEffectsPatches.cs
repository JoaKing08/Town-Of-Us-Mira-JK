using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using TownOfUs;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modifiers.Game.Crewmate;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules;
using TownOfUs.Options;
using TownOfUs.Options.Maps;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

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