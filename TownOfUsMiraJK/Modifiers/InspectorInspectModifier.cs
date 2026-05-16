using MiraAPI.Events;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Events.TouEvents;
using TownOfUsMiraJK.Enums;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class InspectorInspectModifier(PlayerControl inspector) : BaseModifier
{
    public override string ModifierName => "Confess";
    public override bool HideOnUi => true;
    public PlayerControl Inspector { get; } = inspector;

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.InspectorInspect, Inspector, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}