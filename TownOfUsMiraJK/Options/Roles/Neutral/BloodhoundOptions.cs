using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class BloodhoundOptions : AbstractOptionGroup<BloodhoundRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleBloodhound", "Bloodhound");

    [ModdedNumberOption("TouJKOptionBloodhoundKillCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionBloodhoundKillsToBloodlust", 1, 5, 1, MiraNumberSuffixes.None, "0")]
    public float KillsToBloodlust { get; set; } = 3;

    [ModdedNumberOption("TouJKOptionBloodhoundBloodlustCooldown", 2.5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BloodlustCooldown { get; set; } = 10f;

    [ModdedNumberOption("TouJKOptionBloodhoundBloodlustDuration", 10f, 180f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BloodlustDuration { get; set; } = 30f;

    [ModdedToggleOption("TouJKOptionBloodhoundCanVent")]
    public bool CanVent { get; set; } = true;
}
