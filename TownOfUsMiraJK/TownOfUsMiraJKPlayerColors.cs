using MiraAPI.Colors;
using UnityEngine;

namespace TownOfUsMiraJK;

[RegisterCustomColors]
public static class TownOfUsMiraJKPlayerColors
{
    public static CustomColor Grayscale { get; } = new("Grayscale",
        new Color32(0, 0, 0, byte.MaxValue))
    {
        ColorBrightness = CustomColorBrightness.Darker
    };
    public static CustomColor Fire { get; } = new("Fire",
        new Color32(0, 0, 0, byte.MaxValue))
    {
        ColorBrightness = CustomColorBrightness.Lighter
    };
    public static CustomColor Galaxy { get; } = new("Galaxy",
        new Color32(0, 0, 0, byte.MaxValue))
    {
        ColorBrightness = CustomColorBrightness.Darker
    };
}
