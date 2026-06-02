using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class BerserkerOptions : AbstractOptionGroup<BerserkerRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleBerserker", "Berserker");

    [ModdedNumberOption("TouJKOptionBerserkerInstantWar", 0, 100f, 10f, MiraNumberSuffixes.Percent)]
    public float WarChance { get; set; } = 0f;

    [ModdedNumberOption("TouJKOptionBerserkerInitialCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    public ModdedNumberOption KillCooldownReduction { get; } = new("TouJKOptionBerserkerCooldownReduction", 5f, 2.5f,
        15f, 1f, "#", "#", MiraNumberSuffixes.Seconds, halfIncrements: true);

    [ModdedToggleOption("TouJKOptionBerserkerCanVent")]
    public bool CanVent { get; set; } = true;

    [ModdedNumberOption("TouJKOptionBerserkerKillsToTransform", 1, 8, 1)]
    public float KillsToTransform { get; set; } = 4;

    [ModdedToggleOption("TouJKOptionBerserkerAnnounceTransformation")]
    public bool AnnounceWar { get; set; } = true;

    [ModdedNumberOption("TouJKOptionBerserkerWarKillCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float WarKillCooldown { get; set; } = 10f;

    [ModdedNumberOption("TouJKOptionBerserkerWarKillingSpreeDuration", 0.05f, 5f, 0.05f, MiraNumberSuffixes.Seconds, "0.00")]
    public float KillingSpree { get; set; } = 1f;

    [ModdedToggleOption("TouJKOptionBerserkerWarCanVent")]
    public bool WarCanVent { get; set; } = true;
}