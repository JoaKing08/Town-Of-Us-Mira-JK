using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Roles.Impostor;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers.Secret;
using TownOfUsMiraJK.Options.Roles.Secret;
using TownOfUsMiraJK.Roles.Secret;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Secret;

public sealed class ShadowVanishButton : TownOfUsRoleButton<ShadowRole>, IAftermathableButton
{
    public override Color TextOutlineColor => Colors.Shadow;
    public override string Name => TouLocale.GetParsed("TouJKRoleShadowVanish", "Vanish");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ShadowOptions>.Instance.VanishCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<ShadowOptions>.Instance.VanishDuration;
    public override LoadableAsset<Sprite> Sprite => SecrAssets.ShadowVanishSprite;

    public override bool ZeroIsInfinite { get; set; } = true;

    public void AftermathHandler()
    {
        if (!EffectActive)
        {
            PlayerControl.LocalPlayer.RpcAddModifier<ShadowVanishModifier>();
        }
        else
        {
            OnEffectEnd();
        }
    }

    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
        Button?.SetDisabled();
        if (EffectActive)
        {
            Timer = Cooldown;
            EffectActive = false;
        }
        else if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
        else
        {
            Timer = Cooldown;
        }
    }

    public override bool CanUse()
    {
        if (HudManager.Instance.Chat.IsOpenOrOpening || MeetingHud.Instance)
        {
            return false;
        }

        if (PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() || PlayerControl.LocalPlayer
                .GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        return (Timer <= 0 && !EffectActive && (!LimitedUses || UsesLeft > 0)) ||
                (EffectActive && Timer <= EffectDuration - 2f);
    }

    protected override void OnClick()
    {
        if (!EffectActive)
        {
            PlayerControl.LocalPlayer.RpcAddModifier<ShadowVanishModifier>();
        }
        else
        {
            OnEffectEnd();
        }
    }

    public override void OnEffectEnd()
    {
        if (!PlayerControl.LocalPlayer.HasModifier<ShadowVanishModifier>())
        {
            return;
        }

        PlayerControl.LocalPlayer.RpcRemoveModifier<ShadowVanishModifier>();
    }
}