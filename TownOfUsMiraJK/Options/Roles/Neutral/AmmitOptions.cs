using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class AmmitOptions : AbstractOptionGroup<AmmitRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleAmmit", "Ammit");

    [ModdedNumberOption("TouJKOptionAmmitDevourCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DevourCooldown { get; set; } = 20f;

    public ModdedNumberOption DevourCooldownIncrease { get; } = new("TouJKOptionAmmitDevourCooldownIncrease", 5f, 2.5f,
        15f, 1f, "#", "#", MiraNumberSuffixes.Seconds, halfIncrements: true);

    [ModdedNumberOption("TouJKOptionAmmitMaxDevoured", 0, 5, 1, MiraNumberSuffixes.None, "0", zeroInfinity: true)]
    public float MaxDevoured { get; set; } = 0;

    public ModdedNumberOption SizePerPerson { get; } = new("TouJKOptionAmmitSizePerPerson", 2.5f, 0f,
        10f, 1f, "#", "#", MiraNumberSuffixes.Percent, halfIncrements: true);

    [ModdedToggleOption("TouJKOptionAmmitCanVent")]
    public bool CanVent { get; set; } = false;
}
