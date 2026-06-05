using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Game.Alliance;

public sealed class NecromancerUndeadModifier : AllianceGameModifier, IColoredModifier
{
    public override string LocaleKey => "Undead";
    public override string ModifierName => TouLocale.Get($"TouJKModifier{LocaleKey}");
    public string ShortName => TouLocale.Get($"TouJKModifier{LocaleKey}ShortName");

    public override string GetDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescription");
    }

    public override string Symbol => "ü";
    public override bool DoesTasks => false;
    public override bool GetsPunished => false;
    public override bool CrewContinuesGame => false;
    public override AlliedFaction TrueFactionType => AlliedFaction.RoleSpecific;

    public override Color FreeplayFileColor => new Color32(220, 220, 220, 255);
    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKRoleIcons.Necromancer;

    public int Priority { get; set; } = -1;
    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override int GetAmountPerGame()
    {
        return 0;
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        if (Player.AmOwner)
        {
            // player is meant to be crewmate, and crewmates don't have a task header!
            TouRoleUtils.ClearTaskHeader(PlayerControl.LocalPlayer);
        }
        if (!Player.HasModifier<BasicGhostModifier>())
        {
            Player.AddModifier<BasicGhostModifier>();
        }

        if (Player.HasModifier<ToBecomeTraitorModifier>())
        {
            Player.RemoveModifier<ToBecomeTraitorModifier>();
        }
    }

    public static bool UndeadVisibilityFlag(PlayerControl player)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var isUndead = PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<NecromancerRole>()) || PlayerControl.LocalPlayer.HasModifier<NecromancerUndeadModifier>();

        return player.AmOwner || player.Data != null && !player.Data.Disconnected && isUndead;
    }

    public override bool? DidWin(GameOverReason reason)
    {
        if (!CustomRoleUtils.GetActiveRolesOfType<NecromancerRole>().Any(x => !x.Player.HasDied()))
        {
            return false;
        }

        var undeadCount = ModifierUtils.GetActiveModifiers<NecromancerUndeadModifier>().Count(x => !x.Player.HasDied()) + 1;

        if (Helpers.GetAlivePlayers().Any(x => (x.IsImpostor() ||
        x.Is(RoleAlignment.NeutralKilling) ||
        (x.Data.Role is ITouCrewRole { IsPowerCrew: true } &&
         !(x.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.CrewContinuesGame) &&
         OptionGroupSingleton<GameMechanicOptions>.Instance.CrewKillersContinue)) &&
         !x.HasModifier<NecromancerUndeadModifier>()))
        {
            return false;
        }

        return undeadCount >= Helpers.GetAlivePlayers().Count - undeadCount;
    }

    public Color ModifierColor => TownOfUsMiraJKColors.Necromancer;
}