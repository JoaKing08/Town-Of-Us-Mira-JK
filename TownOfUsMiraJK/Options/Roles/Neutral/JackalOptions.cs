using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUs.Modules.Localization;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class JackalOptions : AbstractOptionGroup<JackalRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleNecromancer", "Necromancer");

    [ModdedNumberOption("TouJKJackalKillCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 20f;

    [ModdedToggleOption("TouJKJackalCanVent")]
    public bool CanVent { get; set; } = false;

    [ModdedToggleOption("TouJKJackalRecruitsDieTogether")]
    public bool RecruitsDieTogether { get; set; } = true;

    [ModdedToggleOption("TouJKJackalRecruitsSeeJackal")]
    public bool RecruitsSeeJackal { get; set; } = false;
}