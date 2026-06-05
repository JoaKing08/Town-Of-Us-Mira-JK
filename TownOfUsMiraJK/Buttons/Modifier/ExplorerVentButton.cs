using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Usables;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers.Game.Crewmate;
using TownOfUsMiraJK.Options.Modifiers.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Modifier;

public sealed class ExplorerVentButton : TownOfUsTargetButton<Vent>
{
    public override string Name => TranslationController.Instance.GetStringWithDefault(StringNames.VentLabel, "Vent");
    public override BaseKeybind Keybind => Keybinds.VentAction;
    public override Color TextOutlineColor => Colors.Explorer;

    public override float Cooldown =>
        Math.Clamp(OptionGroupSingleton<ExplorerOptions>.Instance.VentCooldown + MapCooldown, 0.001f, 120f);

    public override float EffectDuration => OptionGroupSingleton<ExplorerOptions>.Instance.VentDuration;
    public override int MaxUses => (int)OptionGroupSingleton<ExplorerOptions>.Instance.MaxVents;
    public override LoadableAsset<Sprite> Sprite => ModifAssets.ExplorerVentSprite;
    public override bool ShouldPauseInVent => false;
    public override bool Enabled(RoleBehaviour? role)
    {
        return !Disabled && role?.Player.HasModifier<ExplorerModifier>() == true && role?.Player.HasDied() == false;
    }

    public override Vent? GetTarget()
    {
        return TouRoleUtils.GetClosestUsableVent(true);
    }

    public override bool CanUse()
    {
        var newTarget = GetTarget();
        if (newTarget != Target)
        {
            Target?.SetOutline(false, false);
        }

        Target = IsTargetValid(newTarget) ? newTarget : null;
        SetOutline(true);

        if (HudManager.Instance.Chat.IsOpenOrOpening || MeetingHud.Instance)
        {
            return false;
        }

        if (PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() || PlayerControl.LocalPlayer
                .GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        return (Timer <= 0 && !PlayerControl.LocalPlayer.inVent && Target != null ||
                PlayerControl.LocalPlayer.inVent) && (!LimitedUses || UsesLeft > 0);
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
            Timer = !PlayerControl.LocalPlayer.inVent ? 0.001f : Cooldown;
        }
    }

    protected override void OnClick()
    {
        if (!PlayerControl.LocalPlayer.inVent)
        {
            if (Target != null)
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(Target.Id);
                Target.SetButtons(true);
            }
        }
        else if (Timer != 0)
        {
            OnEffectEnd();
            if (!HasEffect)
            {
                EffectActive = false;
                Timer = Cooldown;
            }
        }
    }

    public override void OnEffectEnd()
    {
        if (PlayerControl.LocalPlayer.inVent)
        {
            _ = Vent.currentVent.CanUse(PlayerControl.LocalPlayer.Data, true, out var couldUse);
            Vent.currentVent.SetButtons(false);
            if (!couldUse)
            {
                Error($"Current vent cannot be exited, finding alternate route.");
                Vent? newVent = null;
                foreach (var closeVent in Vent.currentVent.NearbyVents)
                {
                    if (newVent != null)
                    {
                        break;
                    }
                    var @event = new PlayerCanUseEvent(closeVent.Cast<IUsable>());
                    MiraEventManager.InvokeEvent(@event);

                    if (!@event.IsCancelled)
                    {
                        newVent = closeVent;
                    }
                }

                if (newVent != null)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(newVent.Id);
                }
                else
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                }
            }
            else
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
            }
            UsesLeft--;
            if (LimitedUses)
            {
                Button?.SetUsesRemaining(UsesLeft);
            }
        }
    }

    public override void SetOutline(bool active)
    {
        if (Target != null && !PlayerControl.LocalPlayer.HasDied())
        {
            Target.SetOutline(active, true, PlayerControl.LocalPlayer.Data.Role.TeamColor);
        }
    }
}