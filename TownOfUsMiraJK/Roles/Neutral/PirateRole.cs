using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Collections;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Events;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Roles.Other;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class PirateRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant, IContinuesGame
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.GetParsed("NeutralOutlierTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    public bool IsUnlovable => true;
    public bool ContinuesGame => !Player.HasDied() && OptionGroupSingleton<PirateOptions>.Instance.StallGame && !MetWinCon;

    public int DuelsWon { get; set; }
    public int Priority { get; set; } = 5;

    public bool Won { get; set; }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<JailorRole>());
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string LocaleKey => "Pirate";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Duel", "Duel"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}DuelWikiDescription"),
                    NeutAssets.PirateDuelSprite),
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}DuelMech", "Duel Mechanics"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}DuelMechWikiDescription"),
                    NeutAssets.PirateDuelSprite)
            };
        }
    }

    public Color RoleColor => Colors.Pirate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralOutlier;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.SheriffIntroSound,
        Icon = RoleIcons.Pirate,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool MetWinCon => DuelsWon >= OptionGroupSingleton<PirateOptions>.Instance.DuelsToWin;


    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        TouRoleUtils.ClearTaskHeader(Player);

        if (!Player.HasModifier<BasicGhostModifier>() && MetWinCon)
        {
            Player.AddModifier<BasicGhostModifier>();
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

    [MethodRpc((uint)TownOfUsJKRpc.ChangeDuel, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcChangeDuelOption(PlayerControl player, bool pirate, byte option)
    {
        if (!player.TryGetModifier<PirateDuelModifier>(out var modifier))
        {
            return;
        }
        if (pirate)
        {
            modifier.ChosenOptionPirate = (DuelOption)option;
        }
        else
        {
            modifier.ChosenOption = (DuelOption)option;
        }
    }
    [MethodRpc((uint)TownOfUsJKRpc.DoDuel, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcDoDuel(PlayerControl pirate)
    {
        var pirateRole = pirate.GetRole<PirateRole>();
        var dueledModifier = ModifierUtils.GetActiveModifiers<PirateDuelModifier>(x => x.Pirate.PlayerId == pirate.PlayerId && !x.Player.HasDied()).FirstOrDefault();
        if (pirateRole != null && dueledModifier != null && dueledModifier.Pirate.PlayerId == pirate.PlayerId && !pirate.HasDied())
        {
            var dueled = dueledModifier.Player;
            if (dueledModifier.ChosenOptionPirate == dueledModifier.ChosenOption)
            {
                pirateRole.DuelsWon++;
                if (!dueled.HasModifier<InvulnerabilityModifier>() && pirate.AmOwner)
                {
                    pirate.RpcSpecialMurder(dueled, MeetingCheck.ForMeeting, true, createDeadBody: false, teleportMurderer: false,
                        showKillAnim: false,
                        playKillSound: false,
                        causeOfDeath: "Pirate");
                }
                if (pirate.AmOwner)
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRolePirateDuelWonOwner" + dueledModifier.ChosenOption.ToString()).Replace("<player>", $"{Colors.Pirate.ToTextColor()}{dueled.Data.PlayerName}</color>"),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Pirate.LoadAsset());

                    notif1.AdjustNotification();
                }
                if (dueled.AmOwner)
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRolePirateDuelWonTarget" + dueledModifier.ChosenOption.ToString()),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Pirate.LoadAsset());

                    notif1.AdjustNotification();
                }
            }
            else
            {
                if (pirate.AmOwner)
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRolePirateDuelLostOwner" + dueledModifier.ChosenOptionPirate.ToString() + dueledModifier.ChosenOption.ToString()).Replace("<player>", $"{Colors.Pirate.ToTextColor()}{dueled.Data.PlayerName}</color>"),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Pirate.LoadAsset());

                    notif1.AdjustNotification();
                }
                if (dueled.AmOwner)
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRolePirateDuelLostTarget" + dueledModifier.ChosenOptionPirate.ToString() + dueledModifier.ChosenOption.ToString()),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Pirate.LoadAsset());

                    notif1.AdjustNotification();
                }
            }
        }
    }
}