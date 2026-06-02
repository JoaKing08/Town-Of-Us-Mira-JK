using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfUs.Events.TouEvents;
using TownOfUsMiraJK.Enums;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class WitchMarkModifier(PlayerControl witch) : BaseModifier
{
    public override string ModifierName => "Marked";

    public override bool HideOnUi => true;

    public PlayerControl Witch { get; } = witch;


    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.WitchMark, Witch, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void Update()
    {
        if (Player == null || Witch == null)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        ModifierComponent?.RemoveModifier(this);
    }
}