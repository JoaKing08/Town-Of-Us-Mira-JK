using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class SoulCollectorJKOptions : AbstractOptionGroup<SoulCollectorJKRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleSoulCollector", "Soul Collector");

    [ModdedNumberOption("TouJKSoulCollectorSoulsToTransform", 1, 7, 1)]
    public float SoulsToTransform { get; set; } = 4;

    [ModdedNumberOption("TouJKSoulCollectorReapCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ReapCooldown { get; set; } = 25f;

    [ModdedToggleOption("TouJKSoulCollectorNotifyKills")]
    public bool SoulCollectorArrows { get; set; } = true;

    public ModdedNumberOption SoulCollectorArrowDelay { get; set; } =
        new("TouJKSoulCollectorArrowDelay", 0.5f, 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds, "0.0")
        {
            Visible = () => OptionGroupSingleton<SoulCollectorJKOptions>.Instance.SoulCollectorArrows
        };

    public ModdedNumberOption SoulCollectorArrowDuration { get; set; } =
        new("TouJKSoulCollectorArrowDuration", 10f, 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds, "0.0", zeroInfinity: true)
        {
            Visible = () => OptionGroupSingleton<SoulCollectorJKOptions>.Instance.SoulCollectorArrows
        };

    [ModdedNumberOption("TouJKSoulCollectorArmageddonTimer", 10f, 180f, 5f, MiraNumberSuffixes.Seconds)]
    public float ArmageddonTimer { get; set; } = 45f;
}