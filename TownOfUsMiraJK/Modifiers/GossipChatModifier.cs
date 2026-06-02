using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfUs.Events.TouEvents;
using TownOfUsMiraJK.Enums;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class GossipChatModifier(PlayerControl gossip) : BaseModifier
{
    public override string ModifierName => "Chat";
    public override bool HideOnUi => true;
    public PlayerControl Gossip { get; } = gossip;

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.GossipChat, Gossip, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}