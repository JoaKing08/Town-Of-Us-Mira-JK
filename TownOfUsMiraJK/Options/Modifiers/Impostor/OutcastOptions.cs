using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Events.Modifiers;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Modifiers.Game.Impostor;
using TownOfUsMiraJK.Modifiers.Game.Universal;
using UnityEngine;

namespace TownOfUsMiraJK.Options.Modifiers.Impostor;

public sealed class OutcastOptions : AbstractOptionGroup<OutcastModifier>
{
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.IsClassicRoleAssignment;
    public override string GroupName => TouLocale.Get("TouJKModifierOutcast", "Outcast");
    public override uint GroupPriority => 10;
    public override Color GroupColor => Palette.ImpostorRed;

    [ModdedNumberOption("Kill Cooldown Decrease", 2.5f, 20f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldownDecrease { get; set; } = 10f;
}