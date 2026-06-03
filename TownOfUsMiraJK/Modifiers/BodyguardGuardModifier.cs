using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using Reactor.Utilities.Extensions;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Events.TouEvents;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
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
    public GuardLine GuardLineComp { get; set; }

    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.BodyguardGuard, Bodyguard, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        var distance = OptionGroupSingleton<BodyguardOptions>.Instance.GuardDistance;
        if (Bodyguard.AmOwner && distance > 0)
        {
            GuardLineComp = GuardLine.Create(Player.transform, (int)(360 * distance),
                Colors.Bodyguard, distance * ShipStatus.Instance.MaxLightRadius, distance * -10f);
        }
    }
    public override void OnDeactivate()
    {
        if (GuardLineComp != null)
        {
            GuardLineComp.Destroy();
        }
    }

    public override void Update()
    {
        if (Player == null || Bodyguard == null)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }
        if (Bodyguard.AmOwner && OptionGroupSingleton<BodyguardOptions>.Instance.GuardDistance > 0 && Vector2.Distance(Bodyguard.transform.position, Player.transform.position) >
            OptionGroupSingleton<BodyguardOptions>.Instance.GuardDistance * ShipStatus.Instance.MaxLightRadius)
        {
            CustomButtonSingleton<BodyguardGuardButton>.Instance.ResetCooldownAndOrEffect();
            Player.RpcRemoveModifier<BodyguardGuardModifier>();
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