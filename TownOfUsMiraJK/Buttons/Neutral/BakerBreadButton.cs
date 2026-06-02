using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class BakerBreadButton : TownOfUsRoleButton<BakerRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleBakerBread", "Bread");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Baker;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<BakerOptions>.Instance.BreadCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => NeutAssets.BakerBreadSprite;

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: plr => !plr.HasModifier<BakerFedModifier>() && !plr.IsApocalypse() && !plr.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: plr => !plr.HasModifier<BakerFedModifier>() && !plr.IsApocalypse());
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<BakerFedModifier>();
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Baker Bread: Target is null");
            return;
        }

        BakerRole.RpcGiveBread(PlayerControl.LocalPlayer, Target);
    }
}