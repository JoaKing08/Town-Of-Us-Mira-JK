using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Impostor;

public sealed class PoisonerOptions : AbstractOptionGroup<PoisonerRole>
{
    public override string GroupName => TouLocale.Get("TouJKRolePoisoner", "Poisoner");

    [ModdedNumberOption("TouJKOptionPoisonerPoisonDuration", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PoisonDuration { get; set; } = 15f;

    [ModdedToggleOption("TouJKOptionPoisonerShowPoison")]
    public bool ShowPoison { get; set; } = true;

    public ModdedNumberOption PoisonDelay { get; set; } =
        new("TouJKOptionPoisonerPoisonDelay", 5f, 0f, 120f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")
        {
            Visible = () => OptionGroupSingleton<PoisonerOptions>.Instance.ShowPoison
        };

    [ModdedToggleOption("TouJKOptionPoisonerCanKill")]
    public bool CanKill { get; set; } = true;
}