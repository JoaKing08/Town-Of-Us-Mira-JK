using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;

namespace TownOfUsMiraJK.Options;

public sealed class TouMTweaksOptions : AbstractOptionGroup
{
    public override string GroupName => "Town of Us: Mira Tweaks";
    public override uint GroupPriority => 2;

    [ModdedToggleOption("TouJKOptionProsecutorReveal")]
    public bool ProsecutorReveal { get; set; } = false;

    [ModdedToggleOption("TouJKOptionDeputyReveal")]
    public bool DeputyReveal { get; set; } = false;
}