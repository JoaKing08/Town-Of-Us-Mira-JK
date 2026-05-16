using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Options;
using TownOfUs.Utilities;

namespace TownOfUsMiraJK.Options;

public sealed class RoleJKOptions : AbstractOptionGroup
{
    // TODO: Once hide and seek is possibly implemented as a selectable mode, then this code should be removed.
    public override Func<bool> GroupVisible => () =>
        !(GameOptionsManager.Instance.CurrentGameOptions.GameMode is GameModes.HideNSeek
            or GameModes.SeekFools);
    internal static string[] OptionStrings =
    [
        MiscUtils.GetParsedRoleBucket("None"),
        MiscUtils.GetParsedRoleBucket("27")
    ];

    public override string GroupName => "Role Settings";
    public override uint GroupPriority => 2;
    public bool RoleListEnabled => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.RoleAssignmentType.Value is (int)RoleSelectionMode.RoleList;

    public ModdedEnumOption<RoleListOption> Slot1 { get; } =
        new("Replace Slot 1", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot2 { get; } =
        new("Replace Slot 2", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot3 { get; } =
        new("Replace Slot 3", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot4 { get; } =
        new("Replace Slot 4", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot5 { get; } =
        new("Replace Slot 5", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot6 { get; } =
        new("Replace Slot 6", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot7 { get; } =
        new("Replace Slot 7", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot8 { get; } =
        new("Replace Slot 8", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot9 { get; } =
        new("Replace Slot 9", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot10 { get; } =
        new("Replace Slot 10", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot11 { get; } =
        new("Replace Slot 11", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot12 { get; } =
        new("Replace Slot 12", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot13 { get; } =
        new("Replace Slot 13", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot14 { get; } =
        new("Replace Slot 14", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedEnumOption<RoleListOption> Slot15 { get; } =
        new("Replace Slot 15", RoleListOption.None, OptionStrings)
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.RoleList
        };

    public ModdedNumberOption MinNeutralApocalypse { get; } =
        new("Min Neutral Apocalypse", 0f, 0f, 10f, 1f, MiraNumberSuffixes.None, "0")
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.MinMaxList
        };

    public ModdedNumberOption MaxNeutralApocalypse { get; } =
        new("Max Neutral Apocalypse", 0f, 0f, 10f, 1f, MiraNumberSuffixes.None, "0")
        {
            Visible = () => OptionGroupSingleton<TownOfUs.Options.RoleOptions>.Instance.CurrentRoleDistribution() is RoleDistribution.MinMaxList
        };
}
public enum RoleListOption
{
    None,
    NeutralApocalypse,
}