namespace TownOfUs.Modifiers.Neutral;

public sealed class DemagogueImmunityModifier(byte demId) : PlayerTargetModifier(demId)
{
    public override string ModifierName => "Demagogue Immunity";
}