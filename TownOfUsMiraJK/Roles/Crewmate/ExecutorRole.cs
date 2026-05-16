using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MS.Internal.Xml.XPath;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Patches.Roles;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class ExecutorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable
{
    private MeetingMenu meetingMenu;
    public bool HasExecuted { get; set; }
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string LocaleKey => "Executor";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Aim", "Aim"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}AimWikiDescription"),
                    CrewAssets.ExecutorAimSprite),
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Execute", "Execute"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}ExecuteWikiDescription"),
                    CrewAssets.ExecutorExecuteSprite)
            };
        }
    }

    public Color RoleColor => Colors.Executor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = RoleIcons.Executor,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        IntroSound = TouAudio.DeputyIntroSound
    };
    public bool IsPowerCrew => !HasExecuted;
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (!Player.HasModifier<ExecutorRevealModifier>())
        {
            Player.AddModifier<ExecutorRevealModifier>(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ExecutorRole>()));
        }

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                CrewAssets.ExecutorExecuteSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.90f, 0f, -3f)
            };
        }
    }

    public void Clear()
    {
        var player = ModifierUtils.GetPlayersWithModifier<ExecutorAimedModifier>(x => x.Executor.AmOwner).FirstOrDefault();

        if (player != null && Player.AmOwner)
        {
            player.RpcRemoveModifier<ExecutorAimedModifier>();
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;
        var role = Player.GetRole<ExecutorRole>()!;

        if (!target.HasModifier<InvulnerabilityModifier>())
        {
            Player.RpcSpecialMurder(target, MeetingCheck.ForMeeting, true, createDeadBody: false, teleportMurderer: false,
                showKillAnim: false,
                playKillSound: false,
                causeOfDeath: "Executor");
        }

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
        RpcExecutorExecuted(Player, target.PlayerId);

        Clear();
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead ||
            HasExecuted || voteArea.GetPlayer()?.HasModifier<JailedModifier>() == true ||
            !voteArea.GetPlayer()?.HasModifier<ExecutorAimedModifier>(x => x.Executor.PlayerId == Player.PlayerId) == true || voteArea.GetPlayer()?.IsRole<DemagogueRole>() == true;
    }

    [MethodRpc((uint)TownOfUsJKRpc.ExecutorExecuted)]
    public static void RpcExecutorExecuted(PlayerControl executor, byte target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(executor);
            return;
        }
        var role = executor.Data.Role as ExecutorRole;
        role.HasExecuted = true;
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }

        Clear();
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance,
                Player.AmOwner && !Player.HasDied() && !HasExecuted && !Player.HasModifier<JailedModifier>());
        }
    }
}