using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class CursedSoulOptions : AbstractOptionGroup<CursedSoulRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleCursedSoul", "CursedSoul");

    [ModdedNumberOption("TouJKOptionCursedSoulSoulSwapCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SoulSwapCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionCursedSoulRandomSwapChance", 0, 100, 10f, MiraNumberSuffixes.Percent)]
    public float RandomSwapChance { get; set; } = 50;

    [ModdedToggleOption("TouJKOptionCursedSoulSwapFactionModifier")]
    public bool SwapFactionModifier { get; set; } = true;

    [ModdedToggleOption("TouJKOptionCursedSoulSwapAssassinModifier")]
    public bool SwapAssassinModifier { get; set; } = true;

    [ModdedToggleOption("TouJKOptionCursedSoulSwapWithImpostor")]
    public bool SwapWithImpostor { get; set; } = false;

    [ModdedToggleOption("TouJKOptionCursedSoulSwapWithNeutralKiller")]
    public bool SwapWithNeutralKiller { get; set; } = true;

    [ModdedToggleOption("TouJKOptionCursedSoulSwapWithNeutralApocalypse")]
    public bool SwapWithNeutralApocalypse { get; set; } = false;

    [ModdedToggleOption("TouJKOptionCursedSoulKillOnNonValidSwap")]
    public bool KillOnNonValidSwap { get; set; } = true;

    [ModdedEnumOption("TouJKOptionCursedSoulSwappedPlayerBecomes", typeof(SwappedRole), ["CrewmateKeyword", "TouRoleAmnesiac", "TouRoleSurvivor", "TouRoleMercenary", "TouRoleJester", "TouJKRoleCursedSoul", "TouJKRoleAnarchist"])]
    public SwappedRole SwappedPlayerBecomes { get; set; } = SwappedRole.CursedSoul;
}

public enum SwappedRole
{
    Crew,
    Amnesiac,
    Survivor,
    Mercenary,
    Jester,
    CursedSoul,
    Anarchist
}