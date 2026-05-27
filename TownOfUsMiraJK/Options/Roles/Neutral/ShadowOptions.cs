using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Interfaces;
using TownOfUs.Modules.Localization;
using TownOfUs.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class ShadowOptions : AbstractOptionGroup<ShadowRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleShadow", "Shadow");

    [ModdedNumberOption("TouJKOptionShadowChance", 0, 100, 10f, MiraNumberSuffixes.Percent)]
    public float ShadowChance { get; set; } = 1.5f;

    [ModdedNumberOption("TouJKOptionShadowKillCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionShadowVanishCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float VanishCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionShadowVanishDuration", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float VanishDuration { get; set; } = 10f;
    public ModdedNumberOption VanishOpacity { get; } = new("TouJKOptionShadowVanishOpacity", 5f, 0f,
        10f, 1f, "#", "#", MiraNumberSuffixes.Percent, halfIncrements: true);

    [ModdedNumberOption("TouJKOptionShadowVanishSpeed", 1.00f, 2.5f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float VanishSpeed { get; set; } = 1.25f;

    [ModdedNumberOption("TouJKOptionShadowDarknessCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DarknessCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionShadowDarknessDuration", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DarknessDuration { get; set; } = 10f;

    [ModdedToggleOption("TouJKOptionShadowCanVent")]
    public bool CanVent { get; set; } = true;
}
