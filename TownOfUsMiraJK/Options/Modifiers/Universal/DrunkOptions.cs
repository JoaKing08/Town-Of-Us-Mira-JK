using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Modifiers.Game.Universal;
using UnityEngine;

namespace TownOfUsMiraJK.Options.Modifiers.Universal;

public sealed class DrunkOptions : AbstractOptionGroup<DrunkModifier>
{
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.IsClassicRoleAssignment;
    public override string GroupName => TouLocale.Get("TouJKModifierDrunk", "Drunk");
    public override uint GroupPriority => 10;
    public override Color GroupColor => Colors.Drunk;

    public ModdedToggleOption DrunkExpires { get; set; } = new("Drunk Expires", true);

    public ModdedNumberOption DrunkRounds { get; set; } = new("Drunk Rounds", 3, 1, 15, 1, MiraAPI.Utilities.MiraNumberSuffixes.None)
    {
        Visible = () => OptionGroupSingleton<DrunkOptions>.Instance.DrunkExpires
    };
}