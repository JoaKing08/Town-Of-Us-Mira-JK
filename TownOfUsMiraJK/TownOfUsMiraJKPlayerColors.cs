using MiraAPI.Colors;
using UnityEngine;

namespace TownOfUsMiraJK;

[RegisterCustomColors]
public static class TownOfUsMiraJKPlayerColors
{
    public static CustomColor Grayscale { get; } = new("TouJKGrayscale",
        new Color32(0, 0, 0, byte.MaxValue))
    {
        ColorBrightness = CustomColorBrightness.Darker
    };
    public static CustomColor Fire { get; } = new("TouJKFire",
        new Color32(0, 0, 0, byte.MaxValue))
    {
        ColorBrightness = CustomColorBrightness.Lighter
    };
    public static CustomColor Galaxy { get; } = new("TouJKGalaxy",
        new Color32(0, 0, 0, byte.MaxValue))
    {
        ColorBrightness = CustomColorBrightness.Darker
    };
}
