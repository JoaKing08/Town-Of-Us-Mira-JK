using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class PirateDuelButton : TownOfUsRoleButton<PirateRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRolePirateDuel", "Duel");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsMiraJKColors.Pirate;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<PirateOptions>.Instance.DuelCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => ToUJKNeutAssets.PirateDuelSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<PirateDuelModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Inspector Inspect: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<PirateDuelModifier>(x => x.Pirate.AmOwner);
        players.Do(x => x.RpcRemoveModifier<PirateDuelModifier>());

        Target.RpcAddModifier<PirateDuelModifier>(PlayerControl.LocalPlayer);

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed("TouJKRolePirateDuelNotif").Replace("<player>", $"{TownOfUsMiraJKColors.Pirate.ToTextColor()}{Target.Data.PlayerName}</color>"),
            Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Pirate.LoadAsset());

        notif1.AdjustNotification();
    }
}