using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUsMiraJK.Options.Roles.Impostor;

public sealed class SniperOptions : AbstractOptionGroup<SniperRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleSniper", "Sniper");

    [ModdedNumberOption("TouJKOptionSniperAimCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float AimCooldown { get; set; } = 10f;

    [ModdedToggleOption("TouJKOptionSniperShowArrow")]
    public bool ShowArrow { get; set; } = true;
    public ModdedNumberOption UpdateInterval { get; set; } =
        new("TouJKOptionSniperArrowUpdateInterval", 2.5f, 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds, "0.0")
        {
            Visible = () => OptionGroupSingleton<SniperOptions>.Instance.ShowArrow
        };

    [ModdedToggleOption("TouJKOptionSniperAnnounceShot")]
    public bool AnnounceShot { get; set; } = true;

    public ModdedToggleOption ShowSniper { get; set; } =
        new("TouJKOptionSniperShowSniper", true)
        {
            Visible = () => OptionGroupSingleton<SniperOptions>.Instance.AnnounceShot
        };

    public ModdedToggleOption ShowVictim { get; set; } =
        new("TouJKOptionSniperShowVictim", true)
        {
            Visible = () => OptionGroupSingleton<SniperOptions>.Instance.AnnounceShot
        };

    public ModdedNumberOption ArrowDuration { get; set; } =
        new("TouJKOptionSniperArrowDuration", 1f, 0.25f, 5f, 0.25f, MiraNumberSuffixes.Seconds, "0.0")
        {
            Visible = () => OptionGroupSingleton<SniperOptions>.Instance.AnnounceShot && (OptionGroupSingleton<SniperOptions>.Instance.ShowSniper || OptionGroupSingleton<SniperOptions>.Instance.ShowVictim)
        };

    [ModdedToggleOption("TouJKOptionSniperCanKill")]
    public bool CanKill { get; set; } = true;
}