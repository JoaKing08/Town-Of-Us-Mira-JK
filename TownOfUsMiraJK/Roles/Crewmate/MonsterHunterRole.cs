using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class MonsterHunterRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable
{
    public bool StakesLeft { get; set; }
    public DoomableType DoomHintType => DoomableType.Hunter;
    public string LocaleKey => "MonsterHunter";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Stake", "Stake"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}StakeWikiDescription"),
                    CrewAssets.MonsterHunterStakeSprite)
            };
        }
    }

    public Color RoleColor => Colors.MonsterHunter;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
    public bool IsPowerCrew => StakesLeft; // Always disable end game checks if the monster hunter has stakes

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = RoleIcons.MonsterHunter,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        IntroSound = TouAudio.TrackerIntroSound,
        MaxRoleCount = 1
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = new StringBuilder();
        stringB.AppendLine(TownOfUsPlugin.Culture,
            $"{RoleColor.ToTextColor()}{TouLocale.Get("YouAreA")}<b> {RoleName}.</b></color>");
        stringB.AppendLine(TownOfUsPlugin.Culture,
            $"<size=60%>{TouLocale.Get("Alignment")}: <b>{MiscUtils.GetParsedRoleAlignment(RoleAlignment, true)}</b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(TownOfUsPlugin.Culture, $"{TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription")}");

        return stringB;
    }

    [MethodRpc((uint)TownOfUsJKRpc.WrongStake)]
    public static void RpcWrongStake(PlayerControl monsterHunter, bool hasRemainingStakes)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(monsterHunter);
            return;
        }
        if (monsterHunter.Data.Role is not MonsterHunterRole role)
        {
            Error("RpcWrongStake - Invalid monsterHunter");
            return;
        }

        role.StakesLeft = hasRemainingStakes;
    }
}