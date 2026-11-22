using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Roles.Neutral;
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
    [HideFromIl2Cpp]
    public static List<RoleBehaviour> MonsterRoles
    {
        get
        {
            var types = new List<Type> { typeof(VampireRole), typeof(WerewolfRole) };
            if (TownOfUsMiraJKPlugin.ExtraInstalled)
            {
                types.Add(typeof(TownOfExtra.Roles.Neutral.Evil.PoltergeistRole));
            }
            return CustomRoleManager.AllRoles.Where(x => types.Any(y => y == x.GetType())).ToList();
        }
    }
    [HideFromIl2Cpp]
    public static List<RoleBehaviour> RequiredRoles
    {
        get
        {
            var types = new List<Type> { typeof(VampireRole), typeof(WerewolfRole), typeof(NecromancerRole) };
            if (TownOfUsMiraJKPlugin.ExtraInstalled)
            {
                types.Add(typeof(TownOfExtra.Roles.Neutral.Evil.PoltergeistRole));
            }
            return CustomRoleManager.AllRoles.Where(x => types.Any(y => y == x.GetType())).ToList();
        }
    }
    [HideFromIl2Cpp]
    public static List<BaseModifier> MonsterModifiers
    {
        get
        {
            var types = new List<Type> { typeof(NecromancerUndeadModifier) };
            return ModifierManager.Modifiers.Where(x => types.Any(y => y == x.GetType())).ToList();
        }
    }

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
            var names = MonsterRoles.Select(x => x.GetRoleName()).ToList();
            names.AddRange(MonsterModifiers.Select(x => x.ModifierName));
            names = names.OrderBy(x => x).ToList();
            var last = names[-1];
            names.RemoveAt(names.Count - 1);
            var monsters = names.Join() + TouLocale.Get("TouJKOr") + last;
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Stake", "Stake"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}StakeWikiDescription").Replace("<monsters>", monsters),
                    ToUJKCrewAssets.MonsterHunterStakeSprite)
            };
        }
    }

    public Color RoleColor => TownOfUsMiraJKColors.MonsterHunter;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
    public bool IsPowerCrew => StakesLeft; // Always disable end game checks if the monster hunter has stakes

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = ToUJKRoleIcons.MonsterHunter,
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