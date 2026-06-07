using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Options;
using UnityEngine;

namespace TownOfUsMiraJK.Options.Modifiers;

public sealed class ImpostorModifierJKOptions : AbstractOptionGroup
{
    public override string GroupName => "Impostor Modifiers";
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment;
    public override Color GroupColor => Palette.ImpostorRoleHeaderRed;
    public override bool ShowInModifiersMenu => true;
    public override uint GroupPriority => 3;

    [ModdedNumberOption("TouJKOptionTaskerAmount", 0, 5)]
    public float TaskerAmount { get; set; } = 0;

    public ModdedNumberOption TaskerChance { get; } =
        new("TouJKOptionTaskerChance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<ImpostorModifierJKOptions>.Instance.TaskerAmount > 0
        };

    public ModdedNumberOption OutcastChance { get; } =
        new("TouJKOptionOutcastChance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent);
}