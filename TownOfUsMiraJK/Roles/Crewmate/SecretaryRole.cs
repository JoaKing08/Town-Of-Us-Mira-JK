using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using System.Text;
using TMPro;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;
using UnityEngine.Events;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class SecretaryRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable
{
    [HideFromIl2Cpp] public PlayerVoteArea? StoreButton { get; private set; }
    public int VotesStored { get; set; } = (int)(OptionGroupSingleton<SecretaryOptions>.Instance.MaxVotes <= 0 ? OptionGroupSingleton<SecretaryOptions>.Instance.InitialVotes : MathF.Min(OptionGroupSingleton<SecretaryOptions>.Instance.InitialVotes, OptionGroupSingleton<SecretaryOptions>.Instance.MaxVotes));
    public bool NormalVoteUsed { get; set; }
    public bool StoredVote { get; set; }
    [HideFromIl2Cpp]
    public List<byte> VotedFor { get; set; } = new();
    public int VotesShown { get; set; }

    public void FixedUpdate()
    {
        if (!Player || Player.Data.Role is not SecretaryRole)
        {
            return;
        }

        var meeting = MeetingHud.Instance;

        if (!Player.AmOwner || meeting == null || StoreButton == null)
        {
            return;
        }

        StoreButton.gameObject.SetActive(meeting.state == MeetingHud.VoteStates.NotVoted && !StoredVote && (!NormalVoteUsed || VotesStored > 0));

        if (!StoreButton.gameObject.active)
        {
            return;
        }

        if (meeting.state == MeetingHud.VoteStates.Discussion &&
            meeting.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
        {
            StoreButton.SetDisabled();
        }
        else
        {
            StoreButton.SetEnabled();
        }

        StoreButton.voteComplete = meeting.SkipVoteButton.voteComplete;
    }
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string LocaleKey => "Secretary";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Store", "Store"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}StoreDescription"),
                    ToUJKRoleIcons.Secretary)
            };
        }
    }

    public Color RoleColor => TownOfUsMiraJKColors.Secretary;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;

    public bool IsPowerCrew => true;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = ToUJKRoleIcons.Secretary,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        IntroSound = TouAudio.ProsIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var text = ITownOfUsRole.SetNewTabText(this);
        text.AppendLine(TownOfUsPlugin.Culture, $"{TouLocale.GetParsed($"TouJKRole{LocaleKey}StoredVotes").Replace("<count>", VotesStored.ToString(TownOfUsPlugin.Culture))}");
        return text;
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        StoredVote = false;
        NormalVoteUsed = false;
        VotesShown = 0;
        VotedFor = new();
        Player.GetVoteData().SetRemainingVotes(VotesStored + 1);

        var meeting = MeetingHud.Instance;
        if (!Player.AmOwner || meeting == null)
        {
            return;
        }

        var skip = meeting.SkipVoteButton;
        StoreButton = Instantiate(skip, skip.transform.parent);
        StoreButton.Parent = meeting;
        StoreButton.SetTargetPlayerId(251);
        StoreButton.transform.localPosition = skip.transform.localPosition + new Vector3(0f, -0.17f, 0f);

        StoreButton.gameObject.GetComponentInChildren<TextTranslatorTMP>().Destroy();
        StoreButton.gameObject.GetComponentInChildren<TextMeshPro>().text =
            TouLocale.GetParsed($"TouJKRole{LocaleKey}Store").ToUpperInvariant();
        StoreButton.gameObject.name = "button_storeButton";

        foreach (var plr in meeting.playerStates.AddItem(skip))
        {
            plr.gameObject.GetComponentInChildren<PassiveButton>().OnClick
                .AddListener((UnityAction)(() => StoreButton.ClearButtons()));
        }

        skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
    }

    public void Cleanup()
    {
        StoreButton = null;
        StoredVote = false;
        NormalVoteUsed = false;
        VotedFor = new();
    }

    [MethodRpc((uint)TownOfUsJKRpc.SecretaryStore)]
    public static void RpcStore(PlayerControl plr)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(plr);
            return;
        }
        if (plr.Data.Role is not SecretaryRole secretaryRole)
        {
            return;
        }

        if (!secretaryRole.NormalVoteUsed)
        {
            if (OptionGroupSingleton<SecretaryOptions>.Instance.MaxVotes <= 0 || secretaryRole.VotesStored < OptionGroupSingleton<SecretaryOptions>.Instance.MaxVotes)
            {
                secretaryRole.VotesStored++;
            }
            secretaryRole.NormalVoteUsed = true;
        }
        secretaryRole.StoredVote = true;
        if (!GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes)
        {
            if (OptionGroupSingleton<MonarchOptions>.Instance.ShowKnightedVotes && plr.HasModifier<KnightedModifier>() && secretaryRole.VotedFor.Count == 0)
            {
                for (int i = 0; i < (int)OptionGroupSingleton<MonarchOptions>.Instance.VotesPerKnight + 1; i++)
                {
                    plr.GetVoteData().VoteForPlayer(MeetingHud.Instance.SkipVoteButton.TargetPlayerId);
                }
            }
            else
            {
                plr.GetVoteData().VoteForPlayer(MeetingHud.Instance.SkipVoteButton.TargetPlayerId);
            }
        }
    }
    [MethodRpc((uint)TownOfUsJKRpc.SecretaryVote)]
    public static void RpcVote(PlayerControl plr, byte voted)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(plr);
            return;
        }
        if (plr.Data.Role is not SecretaryRole secretaryRole)
        {
            return;
        }

        if (secretaryRole.VotedFor.Count == 0 && OptionGroupSingleton<MonarchOptions>.Instance.ShowKnightedVotes && plr.HasModifier<KnightedModifier>() && secretaryRole.VotedFor.Count == 0)
        {
            for (int i = 0; i < (int)OptionGroupSingleton<MonarchOptions>.Instance.VotesPerKnight + 1; i++)
            {
                plr.GetVoteData().VoteForPlayer(voted);
            }
        }
        else
        {
            plr.GetVoteData().VoteForPlayer(voted);
        }

        if (secretaryRole.NormalVoteUsed)
        {
            secretaryRole.VotesStored--;
        }
        else
        {
            secretaryRole.NormalVoteUsed = true;
        }
        secretaryRole.VotedFor.Add(voted);
        if (plr.AmOwner)
        {
            var tmpro = secretaryRole.StoreButton?.gameObject.GetComponentInChildren<TextMeshPro>();
            if (tmpro != null)
            {
                tmpro.text = TouLocale.GetParsed($"TouJKRoleSecretaryAbstain").ToUpperInvariant();
            }
        }
    }
}