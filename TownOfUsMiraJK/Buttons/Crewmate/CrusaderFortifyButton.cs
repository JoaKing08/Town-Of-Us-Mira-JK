using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Buttons.Crewmate;

public sealed class CrusaderFortifyButton : TownOfUsRoleButton<CrusaderRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleCrusaderFortify", "Fortify");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Crusader;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<CrusaderOptions>.Instance.FortifyCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<CrusaderOptions>.Instance.FortifyDuration;
    public override int MaxUses => (int)OptionGroupSingleton<CrusaderOptions>.Instance.FortifyMaxUses;
    public override LoadableAsset<Sprite> Sprite => CrewAssets.CrusaderFortifySprite;

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<CrusaderFortifyModifier>();
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

        Target?.RpcAddModifier<CrusaderFortifyModifier>(PlayerControl.LocalPlayer);
    }
}