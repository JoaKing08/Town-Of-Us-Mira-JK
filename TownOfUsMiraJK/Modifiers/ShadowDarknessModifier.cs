using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Anims;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers;

public sealed class ShadowDarknessModifier(PlayerControl player) : DisabledModifier
{
    public override string ModifierName => "Blinded";
    public override bool HideOnUi => true;
    public override float Duration => OptionGroupSingleton<ShadowOptions>.Instance.DarknessDuration;
    public override bool AutoStart => true;
    public PlayerControl Shadow => player;
    public override bool CanUseAbilities => true;
    public override bool CanReport => true;

    public float VisionPerc { get; set; } = 1f;
    public Color ShadowQuadColor { get; set; }

    public override void OnActivate()
    {
        base.OnActivate();
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.ShadowDarkness, Shadow, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);

        VisionPerc = 1f;

        if (Shadow.AmOwner || (PlayerControl.LocalPlayer.HasDied() &&
            OptionGroupSingleton<PostmortemOptions>.Instance.TheDeadKnow))
        {
            Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, Palette.VisorColor);
        }

        if (Player.AmOwner && !Shadow.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleShadowDarknessNotif").Replace("<role>", $"{Colors.Shadow.ToTextColor()}{Shadow.Data.Role.TeamColor}</color>"), Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Shadow.LoadAsset());

            notif1.AdjustNotification();
        }
        ShadowQuadColor = HudManager.Instance.ShadowQuad.material.color;
    }

    public override void OnDeath(DeathReason reason)
    {
        base.OnDeath(reason);

        ModifierComponent!.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        var opts = OptionGroupSingleton<ShadowOptions>.Instance;

        if (PlayerControl.LocalPlayer.IsImpostorAligned())
        {
            Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, Color.black);
        }

        if (TimeRemaining > opts.DarknessDuration - 1f)
        {
            VisionPerc = TimeRemaining - opts.DarknessDuration + 1f;
        }
        else if (TimeRemaining < 1f)
        {
            VisionPerc = 1f - TimeRemaining;
        }
        else
        {
            VisionPerc = 0f;
        }
        HudManager.Instance.ShadowQuad.material.color = Color.Lerp(Colors.Shadow.SetAlpha(ShadowQuadColor.a), ShadowQuadColor, VisionPerc);

        if (Shadow.AmOwner || (PlayerControl.LocalPlayer.HasDied() &&
            OptionGroupSingleton<PostmortemOptions>.Instance.TheDeadKnow))
        {
            Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, Colors.Shadow);
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        VisionPerc = 1f;

        if (Shadow.AmOwner || (PlayerControl.LocalPlayer.HasDied() &&
            OptionGroupSingleton<PostmortemOptions>.Instance.TheDeadKnow))
        {
            Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, Palette.VisorColor);
        }
    }
}