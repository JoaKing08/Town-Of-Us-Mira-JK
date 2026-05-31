using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using PowerTools;
using Reactor.Utilities.Extensions;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modules;
using TownOfUs.Modules.Anims;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
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
    public GameObject GuardLine { get; set; }

    private float _timer = 0;

    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.CrusaderFortify, Crusader, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        if (Crusader.AmOwner && OptionGroupSingleton<CrusaderOptions>.Instance.FortifyDistance > 0)
        {
            GuardLine = GameObject.Instantiate(ToUJKAssets.GuardLine.LoadAsset(), Player.transform);
            GuardLine.transform.SetParent(Player.transform, false);
            UpdateGuardLine();
        }
    }
    public void UpdateGuardLine(float angleOffset = 0)
    {
        var radius = OptionGroupSingleton<CrusaderOptions>.Instance.FortifyDistance * ShipStatus.Instance.MaxLightRadius;
        var lineRenderer = GuardLine.GetComponent<LineRenderer>();
        lineRenderer.positionCount = (int)(360 * OptionGroupSingleton<CrusaderOptions>.Instance.FortifyDistance);
        lineRenderer.SetColors(Colors.Crusader, Colors.Crusader);
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
        if (GuardLine != null)
        {
            _timer += Time.deltaTime;
            UpdateGuardLine(_timer * OptionGroupSingleton<CrusaderOptions>.Instance.FortifyDistance * -10f);
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