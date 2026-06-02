using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class BodyguardRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public override bool IsAffectedByComms => false;
    public DoomableType DoomHintType => DoomableType.Protective;
    public string LocaleKey => "Bodyguard";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public static string ProtectionString = TouLocale.GetParsed("TouJKRoleBodyguardTabProtecting");

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        ProtectionString = TouLocale.GetParsed("TouJKRoleBodyguardTabProtecting");
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);

        var protectedPlayer = ModifierUtils.GetPlayersWithModifier<BodyguardGuardModifier>(x => x.Bodyguard.AmOwner).FirstOrDefault();
        if (protectedPlayer != null)
        {
            stringB.AppendLine(TownOfUsPlugin.Culture, $"\n<b>{ProtectionString.Replace("<player>", protectedPlayer.Data.PlayerName)}</b>");
        }

        return stringB;
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Guard", "Guard"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}GuardWikiDescription"),
                    CrewAssets.BodyguardGuardSprite)
            };
        }
    }

    public Color RoleColor => Colors.Bodyguard;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.VigiIntroSound,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        Icon = RoleIcons.Bodyguard
    };

    [MethodRpc((uint)TownOfUsJKRpc.TeleportBodyguard)]
    public static void RpcTeleportBodyguard(PlayerControl bodyguard, byte target, byte attacker)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(bodyguard);
            return;
        }
        var targetPosition = (MiscUtils.PlayerById(target)!.transform.position + MiscUtils.PlayerById(attacker)!.transform.position) / 2;
        TransporterRole.Transport(bodyguard, targetPosition);
    }
}