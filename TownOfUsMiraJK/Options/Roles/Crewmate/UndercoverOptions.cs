using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class UndercoverOptions : AbstractOptionGroup<UndercoverRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleUndercover", "Undercover");

    [ModdedToggleOption("TouJKUndercoverImpsKillEachother")]
    public bool ImpsKillEachother { get; set; } = true;

    [ModdedToggleOption("TouJKUndercoverCanBeConcealing")]
    public bool CanBeConcealing { get; set; } = true;

    [ModdedToggleOption("TouJKUndercoverCanBeKilling")]
    public bool CanBeKilling { get; set; } = true;

    [ModdedToggleOption("TouJKUndercoverCanBePower")]
    public bool CanBePower { get; set; } = true;

    [ModdedToggleOption("TouJKUndercoverCanBeSupport")]
    public bool CanBeSupport { get; set; } = false;

    [ModdedToggleOption("TouJKUndercoverCanVent")]
    public bool CanVent { get; set; } = true;

    public ModdedToggleOption CanMoveInVent { get; set; } =
        new("TouJKOptionUndercoverCanMoveInVent", false)
        {
            Visible = () => OptionGroupSingleton<UndercoverOptions>.Instance.CanVent
        };
}