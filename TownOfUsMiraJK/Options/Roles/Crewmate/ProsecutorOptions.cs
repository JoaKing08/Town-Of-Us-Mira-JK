using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUs.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class ProsecutorJKOptions : AbstractOptionGroup<ProsecutorRole>
{
    public override string GroupName => TouLocale.Get("TouRoleProsecutor", "Prosecutor");

    [ModdedToggleOption("TouJKOptionProsecutorReveal")]
    public bool Reveal { get; set; } = false;
}