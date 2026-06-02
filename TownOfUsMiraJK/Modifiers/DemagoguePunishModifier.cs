using MiraAPI.Modifiers;

namespace TownOfUs.Modifiers.Neutral;

public sealed class DemagoguePunishModifier() : BaseModifier
{
    public override string ModifierName => "Punish";

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }
}