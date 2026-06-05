using HarmonyLib;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class WitchMarkButton : TownOfUsRoleButton<WitchRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleWitchMark", "Mark");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsMiraJKColors.Witch;
    public override LoadableAsset<Sprite> Sprite => ToUJKNeutAssets.WitchMarkSprite;
    public override float Cooldown => 0.001f;
    public override float InitialCooldown => 0.001f;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && !Helpers.GetAlivePlayers().Any(x => x.HasModifier<WitchMarkModifier>(y => y.Witch.PlayerId == Role.Player.PlayerId));
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<WitchMarkModifier>(x => x.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId));
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<WitchMarkModifier>(x => x.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Witch Mark: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<WitchMarkModifier>(x => x.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId);
        players.Do(x => x.RpcRemoveModifier<WitchMarkModifier>());

        Target.RpcAddModifier<WitchMarkModifier>(PlayerControl.LocalPlayer);

        CustomButtonSingleton<WitchControlButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<WitchControlButton>.Instance.ResetCooldownAndOrEffect();
        SetActive(false, Role);
    }
}