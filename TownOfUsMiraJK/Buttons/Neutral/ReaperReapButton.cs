using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class ReaperReapButton : TownOfUsRoleButton<ReaperRole, DeadBody>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleReaperReap", "Reap");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override int MaxUses => (int)OptionGroupSingleton<ReaperOptions>.Instance.SoulsToTransform;
    public override Color TextOutlineColor => TownOfUsMiraJKColors.Reaper;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ReaperOptions>.Instance.ReapCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => ToUJKNeutAssets.ReaperReapSprite;

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Reaper Reap: Target is null");
            return;
        }

        ReaperRole.RpcReapSoul(Role.Player, Target);
        if (Role.SoulCount >= OptionGroupSingleton<ReaperOptions>.Instance.SoulsToTransform)
        {
            DeathRole.RpcTriggerDeath(Role.Player);
        }
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer == null ? null : Helpers.GetNearestDeadBodies(PlayerControl.LocalPlayer.GetTruePosition(),
            PlayerControl.LocalPlayer.MaxReportDistance / 4f, Helpers.CreateFilter(Constants.NotShipMask))
            .Find(component => component && !component.Reported && !ReaperRole.ReapedBodies.Contains(component.ParentId));
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return target && target?.Reported == false && !ReaperRole.ReapedBodies.Contains(target.ParentId);
    }
}