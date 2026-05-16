using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using UnityEngine;

namespace TownOfUsMiraJK.Options.Modifiers.Alliance;

public sealed class ProphetOptions : AbstractOptionGroup<ProphetModifier>
{
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.IsClassicRoleAssignment;
    public override string GroupName => TouLocale.Get("TouJKModifierProphet", "Prophet");
    public override uint GroupPriority => 10;
    public override Color GroupColor => Colors.Apocalypse;

    public ModdedToggleOption ProphetReplacesApocalypse { get; set; } = new("Prophet Replaces An Apocalypse Member", true);
}