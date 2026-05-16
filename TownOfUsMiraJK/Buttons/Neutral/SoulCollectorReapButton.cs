using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Networking;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modules;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class SoulCollectorReapButton : TownOfUsRoleButton<SoulCollectorJKRole, DeadBody>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleSoulCollectorReap", "Reap");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override int MaxUses => (int)OptionGroupSingleton<SoulCollectorJKOptions>.Instance.SoulsToTransform;
    public override Color TextOutlineColor => Colors.SoulCollector;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<SoulCollectorJKOptions>.Instance.ReapCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => NeutAssets.SoulCollectorReapSprite;

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("SoulCollector Reap: Target is null");
            return;
        }

        SoulCollectorJKRole.RpcReapSoul(Role.Player, Target);
        if (Role.SoulCount >= OptionGroupSingleton<SoulCollectorJKOptions>.Instance.SoulsToTransform)
        {
            DeathRole.RpcTriggerDeath(Role.Player);
        }
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer == null ? null : Helpers.GetNearestDeadBodies(PlayerControl.LocalPlayer.GetTruePosition(),
            PlayerControl.LocalPlayer.MaxReportDistance / 4f, Helpers.CreateFilter(Constants.NotShipMask))
            .Find(component => component && !component.Reported && !CustomRoleUtils.GetActiveRolesOfType<SoulCollectorJKRole>()
            .Any(x => x.ReapedBodies.Contains(component.ParentId)));
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return target && target?.Reported == false;
    }
}