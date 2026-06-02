using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class AnarchistOptions : AbstractOptionGroup<AnarchistRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleAnarchist", "Anarchist");

    [ModdedNumberOption("TouJKOptionAnarchistMisejectsCount", 1f, 5f, 1f)]
    public float MisejectsCount { get; set; } = 2f;

    [ModdedToggleOption("TouJKOptionAnarchistLearnCrew")]
    public bool LearnCrew { get; set; } = false;

    [ModdedToggleOption("TouJKOptionAnarchistCanButton")]
    public bool CanButton { get; set; } = true;

    [ModdedEnumOption("TouJKOptionAnarchistAnarchistWin", typeof(AnaWinOptions), ["TouJKOptionAnarchistAnarchistWinEndsGame", "TouJKOptionAnarchistAnarchistWinAssaults", "TouJKOptionAnarchistAnarchistWinNothing"])]
    public AnaWinOptions AnarchistWin { get; set; } = AnaWinOptions.Nothing;
}

public enum AnaWinOptions
{
    EndsGame,
    Assaults,
    Nothing
}