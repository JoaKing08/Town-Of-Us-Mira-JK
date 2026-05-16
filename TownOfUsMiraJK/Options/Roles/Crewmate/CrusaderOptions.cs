using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUs.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class CrusaderOptions : AbstractOptionGroup<CrusaderRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleCrusader", "Crusader");

    [ModdedNumberOption("TouJKOptionCrusaderFortifyCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float FortifyCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionCrusaderFortifyDuration", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float FortifyDuration { get; set; } = 30f;

    [ModdedNumberOption("TouJKOptionCrusaderFortifyMaxUses", 1, 15, 1)]
    public float FortifyMaxUses { get; set; } = 5;
}