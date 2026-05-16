using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using System.Collections;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class ExecutorAimButton : TownOfUsKillRoleButton<ExecutorRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleExecutorAim", "Aim");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Executor;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ExecutorOptions>.Instance.AimCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => CrewAssets.ExecutorAimSprite;
    public override int MaxUses => (int)OptionGroupSingleton<ExecutorOptions>.Instance.AimMaxUses;
    public override bool CanUse()
    {
        return base.CanUse() && !Role.HasExecuted;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<ExecutorAimedModifier>(y => y.Executor.PlayerId == PlayerControl.LocalPlayer.PlayerId));
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Executor Aim: Target is null");
            return;
        }

        Target.RpcAddModifier<ExecutorAimedModifier>(PlayerControl.LocalPlayer);

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed("TouJKRoleExecutorAimNotif").Replace("<player>", $"{Colors.Executor.ToTextColor()}{Target.Data.PlayerName}</color>"),
            Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Executor.LoadAsset());

        notif1.AdjustNotification();
    }
}