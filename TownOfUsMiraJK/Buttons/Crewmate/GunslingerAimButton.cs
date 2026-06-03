using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class GunslingerAimButton : TownOfUsRoleButton<GunslingerRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleGunslingerAim", "Aim");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Gunslinger;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<GunslingerOptions>.Instance.AimCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => CrewAssets.GunslingerAimSprite;
    public override int MaxUses => (int)OptionGroupSingleton<GunslingerOptions>.Instance.AimMaxUses;
    public override bool CanUse()
    {
        return base.CanUse() && !Role.HasShot;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<GunslingerAimedModifier>(y => y.Gunslinger.PlayerId == PlayerControl.LocalPlayer.PlayerId));
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Gunslinger Aim: Target is null");
            return;
        }

        Target.RpcAddModifier<GunslingerAimedModifier>(PlayerControl.LocalPlayer);

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed("TouJKRoleGunslingerAimNotif").Replace("<player>", $"{Colors.Gunslinger.ToTextColor()}{Target.Data.PlayerName}</color>"),
            Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Gunslinger.LoadAsset());

        notif1.AdjustNotification();
    }
}