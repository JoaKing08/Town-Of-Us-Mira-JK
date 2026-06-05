using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using TownOfUs.Modules.Localization;
using TownOfUs.Roles.Crewmate;

namespace TownOfUsMiraJK.Options;

public sealed class TouMTweaksOptions : AbstractOptionGroup
{
    public override string GroupName => "Town of Us: Mira Tweaks";
    public override uint GroupPriority => 2;

    [ModdedToggleOption("TouJKOptionProsecutorReveal")]
    public bool ProsecutorReveal { get; set; } = false;
}