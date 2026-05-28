using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class ReaperJKOptions : AbstractOptionGroup<ReaperJKRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleReaper", "Reaper");

    [ModdedNumberOption("TouJKReaperSoulsToTransform", 1, 7, 1)]
    public float SoulsToTransform { get; set; } = 4;

    [ModdedNumberOption("TouJKReaperReapCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ReapCooldown { get; set; } = 25f;

    [ModdedToggleOption("TouJKReaperNotifyKills")]
    public bool ReaperArrows { get; set; } = true;

    public ModdedNumberOption ReaperArrowDelay { get; set; } =
        new("TouJKReaperArrowDelay", 0.5f, 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds, "0.0")
        {
            Visible = () => OptionGroupSingleton<ReaperJKOptions>.Instance.ReaperArrows
        };

    public ModdedNumberOption ReaperArrowDuration { get; set; } =
        new("TouJKReaperArrowDuration", 10f, 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds, "0.0", zeroInfinity: true)
        {
            Visible = () => OptionGroupSingleton<ReaperJKOptions>.Instance.ReaperArrows
        };

    [ModdedNumberOption("TouJKReaperArmageddonTimer", 10f, 180f, 5f, MiraNumberSuffixes.Seconds)]
    public float ArmageddonTimer { get; set; } = 45f;
}