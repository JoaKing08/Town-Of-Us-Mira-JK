using AmongUs.GameOptions;
using MiraAPI.GameOptions;
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

public sealed class PoisonerPoisonButton : TownOfUsRoleButton<PoisonerRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRolePoisonerPoison", "Poison");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Impostor;
    public override float Cooldown => PlayerControl.LocalPlayer.GetKillCooldown();
    public override float EffectDuration => OptionGroupSingleton<PoisonerOptions>.Instance.PoisonDuration;
    public override LoadableAsset<Sprite> Sprite => ToUJKImpAssets.PoisonerPoisonSprite;

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed("TouJKRolePoisonerPoisonOwnerNotif").Replace("<player>", $"{TownOfUsColors.Impostor.ToTextColor()}{Target.Data.PlayerName}</color>").Replace("<time>", OptionGroupSingleton<PoisonerOptions>.Instance.PoisonDuration.ToString("0")),
            Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Poisoner.LoadAsset());

        notif1.AdjustNotification();

        Target.RpcAddModifier<PoisonerPoisonModifier>(PlayerControl.LocalPlayer.PlayerId);
        PlayerControl.LocalPlayer.killTimer = OptionGroupSingleton<PoisonerOptions>.Instance.PoisonDuration + GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown);
    }

    public override PlayerControl? GetTarget()
    {
        return MiscUtils.GetImpostorTarget(Distance);
    }
}