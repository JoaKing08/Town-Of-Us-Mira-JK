using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;

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
}