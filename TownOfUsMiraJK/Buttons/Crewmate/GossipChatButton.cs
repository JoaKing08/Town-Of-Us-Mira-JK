using HarmonyLib;
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

public sealed class GossipChatButton : TownOfUsRoleButton<GossipRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleGossipChat", "Chat");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsMiraJKColors.Gossip;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<GossipOptions>.Instance.ChatCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => ToUJKCrewAssets.GossipChatSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<GossipChatModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Gossip Chat: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<GossipChatModifier>(x => x.Gossip.AmOwner);
        players.Do(x => x.RpcRemoveModifier<GossipChatModifier>());

        Target.RpcAddModifier<GossipChatModifier>(PlayerControl.LocalPlayer);

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed("TouJKRoleGossipChatNotif"),
            Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Gossip.LoadAsset());

        notif1.AdjustNotification();
    }
}