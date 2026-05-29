using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Interfaces;
using TownOfUs.Modules.Localization;
using TownOfUs.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class ManhunterOptions : AbstractOptionGroup<ManhunterRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleManhunter", "Manhunter");

    [ModdedNumberOption("TouJKOptionManhunterKillCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    [ModdedToggleOption("TouJKOptionManhunterContinuesGame")]
    public bool StallGame { get; set; } = true;

    [ModdedNumberOption("TouJKOptionManhunterTargets", 1, 8, 1, MiraNumberSuffixes.None, "0", zeroInfinity: true)]
    public float Targets { get; set; } = 4;
}
