using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class CursedSoulOptions : AbstractOptionGroup<CursedSoulRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleCursedSoul", "CursedSoul");

    [ModdedNumberOption("TouJKCursedSoulSoulSwapCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SoulSwapCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKCursedSoulRandomSwapChance", 0, 100, 10f, MiraNumberSuffixes.Percent)]
    public float RandomSwapChance { get; set; } = 50;

    [ModdedToggleOption("TouJKCursedSoulSwapFactionModifier")]
    public bool SwapFactionModifier { get; set; } = true;

    [ModdedToggleOption("TouJKCursedSoulSwapAssassinModifier")]
    public bool SwapAssassinModifier { get; set; } = true;

    [ModdedToggleOption("TouJKCursedSoulSwapWithImpostor")]
    public bool SwapWithImpostor { get; set; } = false;

    [ModdedToggleOption("TouJKCursedSoulSwapWithNeutralKiller")]
    public bool SwapWithNeutralKiller { get; set; } = true;

    [ModdedToggleOption("TouJKCursedSoulSwapWithNeutralApocalypse")]
    public bool SwapWithNeutralApocalypse { get; set; } = false;

    [ModdedToggleOption("TouJKCursedSoulKillOnNonValidSwap")]
    public bool KillOnNonValidSwap { get; set; } = true;

    [ModdedEnumOption("TouJKCursedSoulSwappedPlayerBecomes", typeof(SwappedRole), ["CrewmateKeyword", "TouRoleAmnesiac", "TouRoleSurvivor", "TouRoleMercenary", "TouRoleJester", "TouJKRoleCursedSoul"])]
    public SwappedRole SwappedPlayerBecomes { get; set; } = SwappedRole.CursedSoul;
}

public enum SwappedRole
{
    Crew,
    Amnesiac,
    Survivor,
    Mercenary,
    Jester,
    CursedSoul
}