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
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class InspectorInspectButton : TownOfUsKillRoleButton<InspectorRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleInspectorInspect", "Inspect");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Inspector;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<InspectorOptions>.Instance.InspectorCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => CrewAssets.InspectorInspectSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<InspectorInspectModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Inspector Inspect: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<InspectorInspectModifier>(x => x.Inspector.AmOwner);
        players.Do(x => x.RpcRemoveModifier<InspectorInspectModifier>());

        Target.RpcAddModifier<InspectorInspectModifier>(PlayerControl.LocalPlayer);

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed("TouJKRoleInspectorInspectNotif").Replace("<player>", $"{Colors.Inspector.ToTextColor()}{Target.Data.PlayerName}</color>"),
            Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Inspector.LoadAsset());

        notif1.AdjustNotification();
    }
}