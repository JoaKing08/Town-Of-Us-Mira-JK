using MiraAPI.Modifiers;

namespace TownOfUsMiraJK.Modifiers;

public sealed class ManhunterTargetModifier : BaseModifier
{
    public override string ModifierName => "Manhunter Target";
    public override bool HideOnUi => true;

    public bool KilledByManhunter { get; set; } = true;
    public bool NewTarget { get; set; }
}