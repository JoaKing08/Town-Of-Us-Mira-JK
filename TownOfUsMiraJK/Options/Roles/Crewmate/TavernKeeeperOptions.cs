using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class TavernKeeperOptions : AbstractOptionGroup<TavernKeeperRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleTavernKeeper", "TavernKeeper");

    [ModdedNumberOption("TouJKOptionTavernKeeperDrinkCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DrinkCooldown { get; set; } = 25f;

    [ModdedToggleOption("TouJKOptionTavernKeeperDrinksResetEveryRound")]
    public bool ResetEveryRound { get; set; } = true;

    [ModdedNumberOption("TouJKOptionTavernKeeperMaxDrinks", 1, 15, 1)]
    public float MaxDrinks { get; set; } = 3;

    [ModdedNumberOption("TouJKOptionTavernKeeperDrinkDuration", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DrinkDuration { get; set; } = 20f;
}