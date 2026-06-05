using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Roles.Impostor;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class GunslingerRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable
{
    private MeetingMenu meetingMenu;
    public bool HasShot { get; set; }
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string LocaleKey => "Gunslinger";
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
                    ToUJKCrewAssets.GunslingerAimSprite),
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Shoot", "Shoot"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}ShootWikiDescription"),
                    ToUJKCrewAssets.GunslingerShootSprite)
            };
        }
    }

    public Color RoleColor => TownOfUsMiraJKColors.Gunslinger;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = ToUJKRoleIcons.Gunslinger,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        IntroSound = TouAudio.DeputyIntroSound
    };
    public bool IsPowerCrew => !HasShot;
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (!Player.HasModifier<GunslingerRevealModifier>())
        {
            Player.AddModifier<GunslingerRevealModifier>(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<GunslingerRole>()));
        }

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                ToUJKCrewAssets.GunslingerShootSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.90f, 0f, -3f)
            };
        }
    }

    public void Clear()
    {
        var player = ModifierUtils.GetPlayersWithModifier<GunslingerAimedModifier>(x => x.Gunslinger.AmOwner).FirstOrDefault();

        if (player != null && Player.AmOwner)
        {
            player.RpcRemoveModifier<GunslingerAimedModifier>();
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;
        var role = Player.GetRole<GunslingerRole>()!;

        if (!target.HasModifier<InvulnerabilityModifier>())
        {
            Player.RpcSpecialMurder(target, MeetingCheck.ForMeeting, true, createDeadBody: false, teleportMurderer: false,
                showKillAnim: false,
                playKillSound: false,
                causeOfDeath: "Gunslinger");
        }

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
        RpcGunslingerShot(Player, target.PlayerId);

        Clear();
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead ||
            HasShot || voteArea.GetPlayer()?.HasModifier<JailedModifier>() == true ||
            !voteArea.GetPlayer()?.HasModifier<GunslingerAimedModifier>(x => x.Gunslinger.PlayerId == Player.PlayerId) == true || voteArea.GetPlayer()?.IsRole<DemagogueRole>() == true;
    }

    [MethodRpc((uint)TownOfUsJKRpc.GunslingerShot)]
    public static void RpcGunslingerShot(PlayerControl Gunslinger, byte target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(Gunslinger);
            return;
        }
        var role = Gunslinger.Data.Role as GunslingerRole;
        role.HasShot = true;
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
                Player.AmOwner && !Player.HasDied() && !HasShot && !Player.HasModifier<JailedModifier>());
        }
    }
}