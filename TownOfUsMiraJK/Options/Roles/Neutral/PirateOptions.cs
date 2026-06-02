using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class PirateOptions : AbstractOptionGroup<PirateRole>
{
    public override string GroupName => TouLocale.Get("TouJKRolePirate", "Pirate");

    [ModdedNumberOption("TouJKPirateDuelsToWin", 1, 7, 1)]
    public float DuelsToWin { get; set; } = 2;

    [ModdedNumberOption("TouJKPirateDuelCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DuelCooldown { get; set; } = 25f;

    [ModdedToggleOption("TouJKPirateContinuesGame")]
    public bool StallGame { get; set; } = true;
}