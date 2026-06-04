using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class WitchOptions : AbstractOptionGroup<WitchRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleWitch", "Witch");

    [ModdedNumberOption("TouJKOptionWitchControlCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ControlCooldown { get; set; } = 25f;

    [ModdedEnumOption("TouJKOptionWitchLearns", typeof(NotEnoughPlayersEffect), ["TouJKOptionWitchLearnsNothing", "TouJKOptionWitchLearnsFaction", "TouJKOptionWitchLearnsAlignment", "TouJKOptionWitchLearnsRole"])]
    public WitchLearns Learns { get; set; } = WitchLearns.Alignment;
}
public enum WitchLearns
{
    Nothing,
    Faction,
    Alignment,
    Role,
}