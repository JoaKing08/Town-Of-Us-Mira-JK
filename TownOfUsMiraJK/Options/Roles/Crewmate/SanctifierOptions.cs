using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class SanctifierOptions : AbstractOptionGroup<SanctifierRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleSanctifier", "Sanctifier");

    [ModdedNumberOption("TouJKOptionSanctifierSanctifyCooldown", 1f, 120f, 1f, MiraNumberSuffixes.Seconds)]
    public float SanctifyCooldown { get; set; } = 25f;

    [ModdedToggleOption("TouJKOptionSanctifierShowSanctify")]
    public bool ShowSanctify { get; set; } = true;

    public ModdedNumberOption ShowSanctifyDelay { get; } = new("TouJKOptionSanctifierShowSanctifyDelay", 10f, 0f, 30f, 1f, MiraNumberSuffixes.None)
    {
        Visible = () => OptionGroupSingleton<SanctifierOptions>.Instance.ShowSanctify
    };

    [ModdedNumberOption("TouJKOptionSanctifierMaxSanctifies", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxSanctifies { get; set; } = 3f;

    [ModdedNumberOption("TouJKOptionSanctifierSanctifySize", 0.05f, 1f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float SanctifySize { get; set; } = 0.5f;

    [ModdedToggleOption("TouJKOptionSanctifierResetSanctify")]
    public bool ResetSanctify { get; set; } = true;

    public ModdedToggleOption TaskUses { get; } = new("TouJKOptionSanctifierGetUsesFromTasks", false)
    {
        Visible = () => !OptionGroupSingleton<SanctifierOptions>.Instance.ResetSanctify
    };
}