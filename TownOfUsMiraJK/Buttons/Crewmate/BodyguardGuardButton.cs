using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Buttons.Crewmate;

public sealed class BodyguardGuardButton : TownOfUsRoleButton<BodyguardRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleBodyguardGuard", "Guard");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Bodyguard;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<BodyguardOptions>.Instance.GuardCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<BodyguardOptions>.Instance.GuardDuration;
    public override LoadableAsset<Sprite> Sprite => CrewAssets.BodyguardGuardSprite;

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<BodyguardGuardModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error($"{Name}: Target is null");
            return;
        }

        Target?.RpcAddModifier<BodyguardGuardModifier>(PlayerControl.LocalPlayer);
    }
}