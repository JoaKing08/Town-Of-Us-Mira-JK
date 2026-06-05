using MiraAPI.LocalSettings;
using MiraAPI.Utilities;
using TownOfUs;
using TownOfUs.Modules.RainbowMod;
using UnityEngine;
using static TownOfUs.Modules.RainbowMod.RainbowUtils;

namespace TownOfUsMiraJK.Modules.RainbowMod;

public static class RainbowJKUtils
{
    public static bool IsForte => LocalSettingsTabSingleton<TownOfUsLocalMiscSettings>.Instance.RainbowColorAsFortegreen.Value;
    public static Color SaturBodyColor { get; private set; } =new Color32(255, 255, 15, 255);
    public static Color SaturShadowColor { get; private set; } =new Color32(254, 0, 0, 255);
    public static Color MonoBodyColor { get; private set; } = new Color32(255, 255, 255, 255);
    public static Color MonoShadowColor { get; private set; } = new Color32(0, 0, 0, 255);
    public static Color StarBodyColor { get; private set; } = new Color32(200, 255, 255, 255);
    public static Color StarShadowColor { get; private set; } = new Color32(0, 64, 64, 255);

    public static Color Grayscale => IsForte ? MonoBodyColor : new HSBColor(0, 0, PP(0.1f, 1f, 0.3f)).ToColor();
    public static Color GrayscaleShadow => IsForte ? MonoShadowColor : Shadow(Grayscale);

    public static Color Fire => IsForte ? SaturBodyColor : new HSBColor(DoublePP(0f, 0.166f, 0.4f, 0.8f, 0.6f), 1, DoublePP(0.5f, 1f, 1f, 2f, 1.5f)).ToColor();
    public static Color FireShadow => IsForte ? SaturShadowColor : Shadow(Fire);

    public static Color Galaxy => IsForte ? StarBodyColor : new HSBColor(DoublePP(0.333f, 0.888f, 0.1f, 0.025f, 0.05f), 1, DoublePP(0.2f, 0.6f, 0.25f, 0.075f, 0.1f)).ToColor();
    public static Color GalaxyShadow => IsForte ? StarShadowColor : Shadow(Galaxy);

    public static float DoublePP(float min, float max, float mula, float mulb, float mulc)
    {
        return Mathf.Lerp(PP(min, max, mula), PP(min, max, mulb), PP(0, 1, mulc));
    }

    public static void SetGrayscale(Renderer rend)
    {
        rend.material.SetColor(ShaderID.BackColor, GrayscaleShadow);
        rend.material.SetColor(ShaderID.BodyColor, Grayscale);
        rend.material.SetColor(ShaderID.VisorColor, Palette.VisorColor);
    }

    public static void SetFire(Renderer rend)
    {
        rend.material.SetColor(ShaderID.BackColor, FireShadow);
        rend.material.SetColor(ShaderID.BodyColor, Fire);
        rend.material.SetColor(ShaderID.VisorColor, Palette.VisorColor);
    }

    public static void SetGalaxy(Renderer rend)
    {
        rend.material.SetColor(ShaderID.BackColor, GalaxyShadow);
        rend.material.SetColor(ShaderID.BodyColor, Galaxy);
        rend.material.SetColor(ShaderID.VisorColor, Palette.VisorColor);
    }

    public static Color SetBasicGrayscale()
    {
        return Grayscale;
    }

    public static Color SetBasicFire()
    {
        return Fire;
    }

    public static Color SetBasicGalaxy()
    {
        return Galaxy;
    }

    public static bool IsGrayscale(int id)
    {
        try
        {
            return Palette.ColorNames[id] == TownOfUsMiraJKPlayerColors.Grayscale.Name;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsFire(int id)
    {
        try
        {
            return Palette.ColorNames[id] == TownOfUsMiraJKPlayerColors.Fire.Name;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsGalaxy(int id)
    {
        try
        {
            return Palette.ColorNames[id] == TownOfUsMiraJKPlayerColors.Galaxy.Name;
        }
        catch
        {
            return false;
        }
    }
}