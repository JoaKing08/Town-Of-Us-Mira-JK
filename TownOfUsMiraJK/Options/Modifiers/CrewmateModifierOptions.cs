using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Options;
using UnityEngine;

namespace TownOfUsMiraJK.Options.Modifiers;

public sealed class CrewmateModifierJKOptions : AbstractOptionGroup
{
    public override string GroupName => "Crewmate Modifiers";
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment;
    public override Color GroupColor => Palette.CrewmateRoleHeaderBlue;
    public override bool ShowInModifiersMenu => true;
    public override uint GroupPriority => 2;

    [ModdedNumberOption("Explorer Amount", 0, 5)]
    public float ExplorerAmount { get; set; } = 0;

    public ModdedNumberOption ExplorerChance { get; } =
        new("Explorer Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<CrewmateModifierJKOptions>.Instance.ExplorerAmount > 0
        };
}