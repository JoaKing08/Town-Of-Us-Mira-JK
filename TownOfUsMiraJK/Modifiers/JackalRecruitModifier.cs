using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using TownOfUs.Events;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Options.Modifiers;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Roles;
using TownOfUs.Roles.Other;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Modifiers;
using TownOfUsMiraJK.Options.Modifiers.Alliance;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Game.Alliance;

public sealed class JackalRecruitModifier : AllianceGameModifier, IColoredModifier
{
    public override string LocaleKey => "Recruit";
    public override string ModifierName => TouLocale.Get($"TouJKModifier{LocaleKey}");
    public string ShortName => TouLocale.Get($"TouJKModifier{LocaleKey}ShortName");

    public override string GetDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescription");
    }

    public override string Symbol => "ƒ";
    public override bool DoesTasks => false;
    public override bool GetsPunished => false;
    public override bool CrewContinuesGame => false;
    public override AlliedFaction TrueFactionType => AlliedFaction.RoleSpecific;

    public override Color FreeplayFileColor => new Color32(220, 220, 220, 255);
    public override LoadableAsset<Sprite>? ModifierIcon => RoleIcons.Jackal;

    public int Priority { get; set; } = -1;
    public List<CustomButtonWikiDescription> Abilities { get; } = [];
    public JackalRecruitModifier? OtherRecruit => ModifierUtils.GetActiveModifiers<JackalRecruitModifier>(x => x.Player.PlayerId != Player.PlayerId).FirstOrDefault();

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

    public static bool RecruitVisibilityFlag(PlayerControl player)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var isRecruit = PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<JackalRole>()) || PlayerControl.LocalPlayer.HasModifier<JackalRecruitModifier>();

        return player.AmOwner || player.Data != null && !player.Data.Disconnected && isRecruit;
    }

    public override bool? DidWin(GameOverReason reason)
    {
        var recruitCount = ModifierUtils.GetActiveModifiers<JackalRecruitModifier>().Count(x => !x.Player.HasDied()) + Helpers.GetAlivePlayers().Count(x => x.Is((RoleTypes)RoleId.Get<JackalRole>()));

        if (Helpers.GetAlivePlayers().Any(x => (x.IsImpostor() ||
        x.Is(RoleAlignment.NeutralKilling) ||
        (x.Data.Role is ITouCrewRole { IsPowerCrew: true } &&
         !(x.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.CrewContinuesGame) &&
         OptionGroupSingleton<GameMechanicOptions>.Instance.CrewKillersContinue)) &&
         !x.HasModifier<JackalRecruitModifier>()))
        {
            return false;
        }

        return recruitCount >= Helpers.GetAlivePlayers().Count - recruitCount;
    }

    public Color ModifierColor => Colors.Jackal;
}