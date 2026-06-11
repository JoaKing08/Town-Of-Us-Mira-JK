using HarmonyLib;
using TownOfUs.Modules.RainbowMod;

namespace TownOfUsMiraJK.Modules.RainbowMod;

[HarmonyPatch(typeof(RainbowBehaviour), nameof(RainbowBehaviour.Update))]
public static class RainbowBehaviourPatch
{
    public static void Postfix(RainbowBehaviour __instance)
    {
        if (__instance.Renderer == null)
        {
            return;
        }

        if (RainbowJKUtils.IsGrayscale(__instance.Id))
        {
            RainbowJKUtils.SetGrayscale(__instance.Renderer);
        }

        if (RainbowJKUtils.IsFire(__instance.Id))
        {
            RainbowJKUtils.SetFire(__instance.Renderer);
        }

        if (RainbowJKUtils.IsGalaxy(__instance.Id))
        {
            RainbowJKUtils.SetGalaxy(__instance.Renderer);
        }
    }
}

[HarmonyPatch(typeof(BasicRainbowBehaviour), nameof(BasicRainbowBehaviour.Update))]
public static class BasicRainbowBehaviourPatch
{
    public static void Postfix(BasicRainbowBehaviour __instance)
    {
        if (__instance.Renderer == null)
        {
            return;
        }

        if (RainbowJKUtils.IsGrayscale(__instance.Id))
        {
            __instance.Renderer.color = RainbowJKUtils.SetBasicGrayscale();
        }

        if (RainbowJKUtils.IsFire(__instance.Id))
        {
            __instance.Renderer.color = RainbowJKUtils.SetBasicFire();
        }

        if (RainbowJKUtils.IsGalaxy(__instance.Id))
        {
            __instance.Renderer.color = RainbowJKUtils.SetBasicGalaxy();
        }
    }
}