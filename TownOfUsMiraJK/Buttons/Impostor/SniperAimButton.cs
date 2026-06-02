using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Roles.Impostor;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Impostor;

public sealed class SniperAimButton : TownOfUsRoleButton<SniperRole, PlayerControl>
{
    public PlayerControl Targeted => ModifierUtils.GetPlayersWithModifier<SniperTargetModifier>(x => x.Owner.PlayerId == PlayerControl.LocalPlayer.PlayerId).FirstOrDefault();
    public override string Name => TouLocale.GetParsed("TouJKRoleSniperAim", "Aim");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<SniperOptions>.Instance.AimCooldown;
    public override LoadableAsset<Sprite> Sprite => ImpAssets.SniperAimSprite;
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Targeted == null;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed("TouJKRoleSniperAimNotif").Replace("<player>", $"{TownOfUsColors.Impostor.ToTextColor()}{Target.Data.PlayerName}</color>"),
            Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Sniper.LoadAsset());

        notif1.AdjustNotification();

        Target.RpcAddModifier<SniperTargetModifier>(PlayerControl.LocalPlayer, TownOfUsColors.Impostor, OptionGroupSingleton<SniperOptions>.Instance.UpdateInterval.Value);
        SetActive(false, Role);
        CustomButtonSingleton<SniperShootButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<SniperShootButton>.Instance.ResetCooldownAndOrEffect();
    }

    public override PlayerControl? GetTarget()
    {
        return MiscUtils.GetImpostorTarget(Distance);
    }
}