using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class UndercoverOptions : AbstractOptionGroup<UndercoverRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleUndercover", "Undercover");

    [ModdedToggleOption("TouJKOptionUndercoverImpsKillEachother")]
    public bool ImpsKillEachother { get; set; } = true;

    [ModdedToggleOption("TouJKOptionUndercoverCanBeConcealing")]
    public bool CanBeConcealing { get; set; } = true;

    [ModdedToggleOption("TouJKOptionUndercoverCanBeKilling")]
    public bool CanBeKilling { get; set; } = true;

    [ModdedToggleOption("TouJKOptionUndercoverCanBePower")]
    public bool CanBePower { get; set; } = true;

    [ModdedToggleOption("TouJKOptionUndercoverCanBeSupport")]
    public bool CanBeSupport { get; set; } = false;

    [ModdedToggleOption("TouJKOptionUndercoverCanVent")]
    public bool CanVent { get; set; } = true;

    public ModdedToggleOption CanMoveInVent { get; set; } =
        new("TouJKOptionUndercoverCanMoveInVent", false)
        {
            Visible = () => OptionGroupSingleton<UndercoverOptions>.Instance.CanVent
        };
}