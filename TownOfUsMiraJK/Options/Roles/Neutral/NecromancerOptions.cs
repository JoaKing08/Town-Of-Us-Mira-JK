using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class NecromancerOptions : AbstractOptionGroup<NecromancerRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleNecromancer", "Necromancer");

    [ModdedNumberOption("TouJKOptionNecromancerMaxUndead", 1, 5, 1)]
    public float MaxUndead { get; set; } = 3;

    [ModdedNumberOption("TouJKOptionNecromancerReanimateCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ReanimateCooldown { get; set; } = 25f;

    [ModdedToggleOption("TouJKOptionNecromancerNotifyKills")]
    public bool NecromancerArrows { get; set; } = false;

    public ModdedNumberOption NecromancerArrowDelay { get; set; } =
        new("TouJKOptionNecromancerArrowDelay", 0.5f, 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds, "0.0")
        {
            Visible = () => OptionGroupSingleton<NecromancerOptions>.Instance.NecromancerArrows
        };

    public ModdedNumberOption NecromancerArrowDuration { get; set; } =
        new("TouJKOptionNecromancerArrowDuration", 10f, 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds, "0.0", zeroInfinity: true)
        {
            Visible = () => OptionGroupSingleton<NecromancerOptions>.Instance.NecromancerArrows
        };
}