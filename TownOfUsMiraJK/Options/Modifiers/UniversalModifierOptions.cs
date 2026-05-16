using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Options;
using UnityEngine;

namespace TownOfUsMiraJK.Options.Modifiers;

public sealed class UniversalModifierJKOptions : AbstractOptionGroup
{
    public override string GroupName => "Universal Modifiers";
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment;
    public override bool ShowInModifiersMenu => true;
    public override uint GroupPriority => 0;

    [ModdedNumberOption("Drunk Amount", 0, 5)]
    public float DrunkAmount { get; set; } = 0;

    public ModdedNumberOption DrunkChance { get; } = new("Drunk Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<UniversalModifierJKOptions>.Instance.DrunkAmount > 0
    };
}