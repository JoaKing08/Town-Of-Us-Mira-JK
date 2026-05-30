using MiraAPI.Modifiers;

namespace TownOfUsMiraJK.Modifiers
{
    public sealed class WitchRevealModifier()
    : BaseModifier
    {
        public override string ModifierName => "Witch Learns";
        public override bool HideOnUi => true;
    }
}
