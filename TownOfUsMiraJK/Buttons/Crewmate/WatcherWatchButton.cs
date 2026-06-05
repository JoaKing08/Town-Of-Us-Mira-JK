using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class WatcherWatchButton : TownOfUsRoleButton<WatcherRole>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleWatcherWatch", "Watch");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsMiraJKColors.Watcher;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<WatcherOptions>.Instance.WatchCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<WatcherOptions>.Instance.WatchDuration;
    public override LoadableAsset<Sprite> Sprite => ToUJKCrewAssets.WatcherWatchSprite;

    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
        Button?.SetDisabled();
        if (!EffectActive && HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
    }

    protected override void OnClick()
    {
        if (!EffectActive)
        {
            PlayerControl.LocalPlayer.RpcAddModifier<WatcherWatchModifier>();
        }
    }

    public override void OnEffectEnd()
    {
        PlayerControl.LocalPlayer.RpcRemoveModifier<WatcherWatchModifier>();
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

        return Timer <= 0 && !EffectActive;
    }
}