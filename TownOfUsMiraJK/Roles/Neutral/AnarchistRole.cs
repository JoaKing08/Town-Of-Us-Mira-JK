using System.Collections;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Roles.Other;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;
using Random = System.Random;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class AnarchistRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (!playerControl.AmOwner)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.GetParsed("NeutralEvilTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    public List<PlayerControl> Misejected { get; set; } = new();
    public int Misejects => Misejected.Count;
    public bool AboutToWin { get; set; }

    [HideFromIl2Cpp] public List<(byte suspect, byte voter)> Voters { get; set; } = [];

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<GossipRole>());
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string LocaleKey => "Anarchist";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => Colors.Anarchist;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralEvil;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.TribunalSound,
        Icon = RoleIcons.Anarchist,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };



    public bool MetWinCon => Misejects >= OptionGroupSingleton<AnarchistOptions>.Instance.MisejectsCount;

    public bool WinConditionMet()
    {
        if (Player.HasDied())
        {
            return false;
        }

        return OptionGroupSingleton<AnarchistOptions>.Instance.AnarchistWin is AnaWinOptions.EndsGame && MetWinCon;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (!OptionGroupSingleton<AnarchistOptions>.Instance.CanButton)
        {
            player.RemainingEmergencies = 0;
        }
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>()!;
        return console == null || console.AllowImpostor;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return MetWinCon;
    }
}