using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs.Events.TouEvents;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Modifiers.Neutral;

public sealed class WatcherWatchModifier() : BaseModifier, IVisualAppearance
{
    public override string ModifierName => "Watching";

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        appearance.Speed = 0;
        return appearance;
    }
    public override void OnActivate()
    {
        base.OnActivate();
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.WatcherWatch, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        if (Player.AmOwner) AdjustCameraSize(3f * OptionGroupSingleton<WatcherOptions>.Instance.WatchVisionMult, false);
    }
    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (Player.AmOwner) AdjustCameraSize(3f, true);
    }

    private static void RefreshUIAnchors()
    {
        ResolutionManager.ResolutionChanged.Invoke(
            (float)Screen.width / Screen.height,
            Screen.width,
            Screen.height,
            Screen.fullScreen
        );

        foreach (var ap in UnityEngine.Object.FindObjectsOfType<AspectPosition>())
            ap.AdjustPosition();
    }

    public static void AdjustCameraSize(float size, bool shadowQuad)
    {
        if (!HudManager.InstanceExists)
        {
            return;
        }

        var instance = HudManager.Instance;
        if (Camera.main != null)
            Camera.main.orthographicSize = size;

        if (instance.UICamera != null)
            instance.UICamera.orthographicSize = size;

        if (shadowQuad)
        {
            instance.ShadowQuad.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead);
        }
        else
        {
            instance.ShadowQuad.gameObject.SetActive(false);
        }

        RefreshUIAnchors();
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }
}