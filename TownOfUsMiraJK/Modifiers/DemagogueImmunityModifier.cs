using MiraAPI.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUs.Modifiers.Neutral;

public sealed class DemagogueImmunityModifier(byte demId) : PlayerTargetModifier(demId)
{
    public override string ModifierName => "Demagogue Immunity";
}