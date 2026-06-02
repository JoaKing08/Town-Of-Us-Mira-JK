using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class CoronerOptions : AbstractOptionGroup<CoronerRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleCoroner", "Coroner");

    [ModdedNumberOption("TouJKOptionCoronerAutopsyCooldown", 2.5f, 120f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float AutopsyCooldown { get; set; } = 10f;

    [ModdedNumberOption("TouJKOptionCoronerMaxAutopsy", 1, 8, 1)]
    public float MaxAutopsy { get; set; } = 3;

    [ModdedToggleOption("TouJKOptionCoronerAutopsyInfoDuringRound")]
    public bool InfoDuringRound { get; set; } = true;

    [ModdedNumberOption("TouJKOptionCoronerKillerUpdate", 1f, 30f, 1f, MiraNumberSuffixes.Seconds, "0.0")]
    public float KillerUpdate { get; set; } = 10f;
}