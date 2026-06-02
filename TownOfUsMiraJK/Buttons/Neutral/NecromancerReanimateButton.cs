using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class NecromancerReanimateButton : TownOfUsRoleButton<NecromancerRole, DeadBody>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleNecromancerReanimate", "Reanimate");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override int MaxUses => (int)OptionGroupSingleton<NecromancerOptions>.Instance.MaxUndead;
    public override Color TextOutlineColor => Colors.Necromancer;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<NecromancerOptions>.Instance.ReanimateCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => NeutAssets.NecromancerReanimateSprite;

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Necromancer Reanimate: Target is null");
            return;
        }
        var target = MiscUtils.PlayerById(Target.ParentId);
        if (target == null)
        {
            Error("Necromancer Reanimate: Target playerControl is null");
            return;
        }
        RpcReanimate(PlayerControl.LocalPlayer, target);
    }
    [MethodRpc((uint)TownOfUsJKRpc.NecromancerReanimate, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcReanimate(PlayerControl necromancer, PlayerControl target)
    {
        foreach (var modifier in target.GetModifiers<AllianceGameModifier>())
        {
            target.RemoveModifier(modifier);
        }
        var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == target.PlayerId);
        var targetEmergencies = target.RemainingEmergencies;
        var necroEmergencies = necromancer.RemainingEmergencies;
        ReviveUtilities.RevivePlayer(necromancer, target, body?.transform?.localPosition ?? necromancer.transform.localPosition, target.GetRoleWhenAlive(), Colors.Necromancer, TouLocale.GetParsed("TouJKRoleNecromancerReanimateTargetNotif").Replace("<player>", necromancer.Data.PlayerName).Replace("<role>", $"{Colors.Necromancer.ToTextColor()}{necromancer.Data.Role.GetRoleName()}</color>"), TouLocale.GetParsed("TouJKRoleNecromancerReanimateNotif").Replace("<player>", $"{Colors.Necromancer.ToTextColor()}{target.Data.PlayerName}</color>"), RoleIcons.Necromancer.LoadAsset());
        target.RemainingEmergencies = targetEmergencies;
        necromancer.RemainingEmergencies = necroEmergencies;
        target.AddModifier<NecromancerUndeadModifier>();
        if (body != null)
        {
            UnityEngine.Object.Destroy(body.gameObject);
        }
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer == null ? null : Helpers.GetNearestDeadBodies(PlayerControl.LocalPlayer.GetTruePosition(),
            PlayerControl.LocalPlayer.MaxReportDistance / 4f, Helpers.CreateFilter(Constants.NotShipMask))
            .Find(component => component && !component.Reported && !CustomRoleUtils.GetActiveRolesOfType<ReaperJKRole>()
            .Any(x => x.ReapedBodies.Contains(component.ParentId)));
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return target && target?.Reported == false;
    }
}