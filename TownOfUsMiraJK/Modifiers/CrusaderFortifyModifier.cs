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

public sealed class CrusaderFortifyModifier(PlayerControl crusader) : BaseShieldModifier
{
    public override string ModifierName => "Fortified";
    public override float Duration => OptionGroupSingleton<CrusaderOptions>.Instance.FortifyDuration;
    public override bool AutoStart => true;

    public override bool HideOnUi => true;

    public PlayerControl Crusader { get; } = crusader;
    public GuardLine GuardLineComp { get; set; }

    private float _timer = 0;

    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.CrusaderFortify, Crusader, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        var distance = OptionGroupSingleton<CrusaderOptions>.Instance.FortifyDistance;
        if (Crusader.AmOwner && distance > 0)
        {
            GuardLineComp = GuardLine.Create(Player.transform, (int)(360 * distance),
                Colors.Crusader, distance * ShipStatus.Instance.MaxLightRadius, distance * -10f);
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
        if (Player == null || Crusader == null)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }
        if (Crusader.AmOwner && OptionGroupSingleton<CrusaderOptions>.Instance.FortifyDistance > 0 && Vector2.Distance(Crusader.transform.position, Player.transform.position) >
            OptionGroupSingleton<CrusaderOptions>.Instance.FortifyDistance * ShipStatus.Instance.MaxLightRadius)
        {
            CustomButtonSingleton<CrusaderFortifyButton>.Instance.ResetCooldownAndOrEffect();
            Player.RpcRemoveModifier<CrusaderFortifyModifier>();
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