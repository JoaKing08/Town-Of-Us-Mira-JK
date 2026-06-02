using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Roles.Impostor;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Impostor;

public sealed class SniperShootButton : TownOfUsRoleButton<SniperRole>, IAftermathableButton
{
    public PlayerControl Targeted => ModifierUtils.GetPlayersWithModifier<SniperTargetModifier>(x => x.Owner.PlayerId == PlayerControl.LocalPlayer.PlayerId).FirstOrDefault();
    public override string Name => TouLocale.GetParsed("TouJKRoleSniperShoot", "Shoot");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Impostor;
    public override LoadableAsset<Sprite> Sprite => ImpAssets.SniperShootSprite;
    public override float Cooldown => PlayerControl.LocalPlayer.GetKillCooldown();

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Targeted != null;
    }
    public void AftermathHandler()
    {
        if (Enabled(Role))
        {
            SniperRole.RpcSniperShoot(PlayerControl.LocalPlayer, Targeted);
            SetActive(false, Role);
            CustomButtonSingleton<SniperAimButton>.Instance.SetActive(true, Role);
            CustomButtonSingleton<SniperAimButton>.Instance.ResetCooldownAndOrEffect();
        }
    }

    protected override void OnClick()
    {
        if (Targeted == null)
        {
            Error("Sniper Shoot: Target is null");
            return;
        }

        SniperRole.RpcSniperShoot(PlayerControl.LocalPlayer, Targeted);
        SetActive(false, Role);
        CustomButtonSingleton<SniperAimButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<SniperAimButton>.Instance.ResetCooldownAndOrEffect();
    }
}