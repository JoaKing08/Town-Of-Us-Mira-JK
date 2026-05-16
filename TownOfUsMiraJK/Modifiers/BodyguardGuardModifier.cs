using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using PowerTools;
using Reactor.Utilities.Extensions;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modules;
using TownOfUs.Modules.Anims;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class BodyguardGuardModifier(PlayerControl bodyguard) : BaseShieldModifier
{
    public override string ModifierName => "Guard";
    public override float Duration => OptionGroupSingleton<BodyguardOptions>.Instance.GuardDuration;
    public override bool AutoStart => true;

    public override bool HideOnUi => true;

    public PlayerControl Bodyguard { get; } = bodyguard;


    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.BodyguardGuard, Bodyguard, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void Update()
    {
        if (Player == null || Bodyguard == null)
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