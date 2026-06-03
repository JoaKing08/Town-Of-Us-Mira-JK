using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Modifiers.Game.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Options.Modifiers.Crewmate;

public sealed class ExplorerOptions : AbstractOptionGroup<ExplorerModifier>
{
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.IsClassicRoleAssignment;
    public override string GroupName => TouLocale.Get("TouJKModifierExplorer", "Explorer");
    public override uint GroupPriority => 10;
    public override Color GroupColor => Palette.ImpostorRed;

    [ModdedNumberOption("Vent Cooldown", 0f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float VentCooldown { get; set; } = 25f;

    [ModdedNumberOption("Vent Duration", 0f, 120f, 2.5f, MiraNumberSuffixes.Seconds, zeroInfinity: true)]
    public float VentDuration { get; set; } = 10f;

    [ModdedNumberOption("Max Vents", 0f, 15f, 1f, zeroInfinity: true)]
    public float MaxVents { get; set; } = 5f;
}