using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using System.Collections;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Options;
using TownOfUs.Options.Maps;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class WarKillButton : TownOfUsKillRoleButton<WarRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => TranslationController.Instance.GetStringWithDefault(StringNames.KillLabel, "Kill");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => Colors.War;
    public override float Cooldown => OptionGroupSingleton<BerserkerOptions>.Instance.WarKillCooldown;
    public override LoadableAsset<Sprite> Sprite => NeutAssets.WarKillSprite;
    public override float EffectDuration => OptionGroupSingleton<BerserkerOptions>.Instance.KillingSpree;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public override bool CanUse()
    {
        return base.CanUse() || EffectActive;
    }

    public override bool CanClick()
    {
        return base.CanClick() || EffectActive;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        if (!Target.Data.IsDead && !MarkedTargets.Contains(Target))
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(Target, MeetingCheck.OutsideMeeting);
        }

        Coroutines.Start(CoMarkForDeath(Target));
    }

    public List<PlayerControl> MarkedTargets = new();

    public IEnumerator CoMarkForDeath(PlayerControl player)
    {
        MarkedTargets.Add(player);
        yield return new WaitForSeconds(1f);
        MarkedTargets.Remove(player);
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !MarkedTargets.Contains(x) && !x.IsLover() && (!x.IsApocalypse() || OptionGroupSingleton<LoversOptions>.Instance.LoverKillTeammates));
        }

        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !MarkedTargets.Contains(x) && !x.IsApocalypse());
    }
    public override void ClickHandler()
    {
        if (CanClick() && !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() &&
            !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>())
        {
            if (LimitedUses)
            {
                UsesLeft--;
                Button?.SetUsesRemaining(UsesLeft);
                TownOfUsColors.UseBasic = false;
                if (TextOutlineColor != Color.clear)
                {
                    SetTextOutline(TextOutlineColor);
                    if (Button != null)
                    {
                        Button.usesRemainingSprite.color = TextOutlineColor;
                    }
                }

                TownOfUsColors.UseBasic = LocalSettingsTabSingleton<TownOfUsLocalRoleSettings>.Instance
                    .UseCrewmateTeamColorToggle.Value;
            }

            OnClick();
            if (HasEffect && !EffectActive)
            {
                EffectActive = true;
                Timer = EffectDuration;
            }
            else if (!HasEffect)
            {
                Timer = Cooldown;
            }
        }
    }
}