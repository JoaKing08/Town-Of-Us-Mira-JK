using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class GunslingerOptions : AbstractOptionGroup<GunslingerRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleGunslinger", "Gunslinger");

    [ModdedNumberOption("TouJKOptionGunslingerAimCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float AimCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionGunslingerAimMaxUses", 1, 15, 1)]
    public float AimMaxUses { get; set; } = 5;

    [ModdedToggleOption("TouJKOptionGunslingerReveal")]
    public bool Reveal { get; set; } = false;
}