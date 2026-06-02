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
    public GameObject GuardLine { get; set; }

    private float _timer = 0;

    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.BodyguardGuard, Bodyguard, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        if (Bodyguard.AmOwner && OptionGroupSingleton<BodyguardOptions>.Instance.GuardDistance > 0)
        {
            GuardLine = GameObject.Instantiate(ToUJKAssets.GuardLine.LoadAsset(), Player.transform);
            GuardLine.transform.SetParent(Player.transform, false);
            UpdateGuardLine();
        }
    }
    public void UpdateGuardLine(float angleOffset = 0)
    {
        var radius = OptionGroupSingleton<BodyguardOptions>.Instance.GuardDistance * ShipStatus.Instance.MaxLightRadius;
        var lineRenderer = GuardLine.GetComponent<LineRenderer>();
        lineRenderer.positionCount = (int)(360 * OptionGroupSingleton<BodyguardOptions>.Instance.GuardDistance);
        lineRenderer.SetColors(Colors.Bodyguard, Colors.Bodyguard);
        for (float i = 0; i < lineRenderer.positionCount; i++)
        {
            var angle = i + angleOffset;
            var position = GuardLine.transform.position;
            position.x += Mathf.Cos(angle * Mathf.PI * 2f / lineRenderer.positionCount) * radius;
            position.y += Mathf.Sin(angle * Mathf.PI * 2f / lineRenderer.positionCount) * radius;
            lineRenderer.SetPosition((int)i, position);
        }
    }
    public override void OnDeactivate()
    {
        if (GuardLine != null)
        {
            GuardLine.Destroy();
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
        if (GuardLine != null)
        {
            _timer += Time.deltaTime;
            UpdateGuardLine(_timer * OptionGroupSingleton<BodyguardOptions>.Instance.GuardDistance * -10f);
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