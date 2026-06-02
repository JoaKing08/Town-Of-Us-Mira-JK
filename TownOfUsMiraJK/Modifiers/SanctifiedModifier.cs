namespace TownOfUs.Modifiers.Crewmate;

public sealed class SanctifiedModifier() : BaseShieldModifier
{
    public override string ModifierName => "Sanctified";

    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        ModifierComponent?.RemoveModifier(this);
    }
}