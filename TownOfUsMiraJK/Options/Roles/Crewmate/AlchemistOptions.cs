using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class AlchemistOptions : AbstractOptionGroup<AlchemistRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleAlchemist", "Alchemist");

    [ModdedNumberOption("TouJKOptionAlchemistBrewCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BrewCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionAlchemistPotionCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PotionCooldown { get; set; } = 10f;

    [ModdedNumberOption("TouJKOptionAlchemistPotionDelay", 5f, 30f, 1f, MiraNumberSuffixes.Seconds)]
    public float PotionDelay { get; set; } = 10f;

    [ModdedNumberOption("TouJKOptionAlchemistIngredientChoices", 2f, 5f, 1f)]
    public float IngredientChoices { get; set; } = 3f;

    [ModdedToggleOption("TouJKOptionAlchemistAllowSelf")]
    public bool AllowSelf { get; set; } = true;

    [ModdedNumberOption("TouJKOptionAlchemistPotionDurationShort", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PotionDurationShort { get; set; } = 10f;

    [ModdedNumberOption("TouJKOptionAlchemistPotionDurationLong", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PotionDurationLong { get; set; } = 30f;

    [ModdedNumberOption("TouJKOptionAlchemistSpeedPotion", 1.05f, 2.5f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float SpeedPotion { get; set; } = 1.5f;

    [ModdedNumberOption("TouJKOptionAlchemistSlownessPotion", 0.25f, 0.95f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float SlownessPotion { get; set; } = 0.75f;

    [ModdedNumberOption("TouJKOptionAlchemistEnergyPotion", 1.05f, 3f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float EnergyPotion { get; set; } = 2f;

    [ModdedNumberOption("TouJKOptionAlchemistBlindnessPotion", 0.0f, 0.95f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float BlindnessPotion { get; set; } = 0.25f;

    [ModdedNumberOption("TouJKOptionAlchemistWorkPotion", 1f, 5f, 1f)]
    public float WorkPotion { get; set; } = 2f;

    [ModdedNumberOption("TouJKOptionAlchemistInfluencePotion", 1f, 3f, 1f)]
    public float InfluencePotion { get; set; } = 1f;

    [ModdedNumberOption("TouJKOptionAlchemistPerceptionPotion", 1.05f, 2.5f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float PerceptionPotion { get; set; } = 1.5f;
}