using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfUs.Options;
using UnityEngine;

namespace TownOfUsMiraJK.Options.Modifiers;

public sealed class AllianceModifierJKOptions : AbstractOptionGroup
{
    public override string GroupName => "Alliance Modifiers";
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment;
    public override Color GroupColor => Color.white;
    public override bool ShowInModifiersMenu => true;
    public override uint GroupPriority => 0;

    [ModdedNumberOption("TouJKOptionProphetChance", 0, 100, 10f, MiraNumberSuffixes.Percent)]
    public float ProphetChance { get; set; } = 0;
}