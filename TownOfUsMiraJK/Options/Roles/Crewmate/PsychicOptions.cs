using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class PsychicOptions : AbstractOptionGroup<PsychicRole>
{
    public override string GroupName => TouLocale.Get("TouJKRolePsychic", "Psychic");

    [ModdedNumberOption("TouJKPsychicMindscanRange", 0.25f, 5f, 0.25f, MiraNumberSuffixes.Multiplier)]
    public float MindscanRange { get; set; } = 1;

    [ModdedNumberOption("TouJKPsychicMindscanCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float MindscanCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKPsychicMindscanCount", 0f, 5f, 1f)]
    public float MindscanCount { get; set; } = 3f;

    [ModdedNumberOption("TouJKPsychicMindscanSucceedChance", 0f, 100f, 10f, MiraNumberSuffixes.Percent)]
    public float MindscanSucceedChance { get; set; } = 100f;

    [ModdedEnumOption("TouJKPsychicMindscanVisibility", typeof(PsychicSees), ["TouJKPsychicMindscanVisibilityFaction", "TouJKPsychicMindscanVisibilityAlignment", "TouJKPsychicMindscanVisibilityRole"])]
    public PsychicSees MindscanVisibility { get; set; } = PsychicSees.Faction;

    [ModdedNumberOption("TouJKPsychicInvis", 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Invis { get; set; } = 10f;
}
public enum PsychicSees
{
    Faction,
    Alignment,
    Role
}