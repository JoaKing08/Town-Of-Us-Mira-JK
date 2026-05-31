using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUs.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class BodyguardOptions : AbstractOptionGroup<BodyguardRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleBodyguard", "Bodyguard");

    [ModdedNumberOption("TouJKOptionBodyguardGuardCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float GuardCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionBodyguardGuardDuration", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float GuardDuration { get; set; } = 30f;

    [ModdedNumberOption("TouJKOptionBodyguardGuardDistance", 0f, 5f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00", true)]
    public float GuardDistance { get; set; } = 1.5f;
}