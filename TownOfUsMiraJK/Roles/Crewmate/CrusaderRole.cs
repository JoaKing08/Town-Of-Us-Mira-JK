using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class CrusaderRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public override bool IsAffectedByComms => false;
    public DoomableType DoomHintType => DoomableType.Relentless;
    public string LocaleKey => "Crusader";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public static string ProtectionString = TouLocale.GetParsed("TouJKRoleCrusaderTabProtecting");

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        ProtectionString = TouLocale.GetParsed("TouJKRoleCrusaderTabProtecting");
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);

        var protectedPlayer = ModifierUtils.GetPlayersWithModifier<CrusaderFortifyModifier>(x => x.Crusader.AmOwner).FirstOrDefault();
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Fortify", "Fortify"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}FortifyWikiDescription"),
                    CrewAssets.CrusaderFortifySprite)
            };
        }
    }

    public Color RoleColor => Colors.Crusader;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.WarlockIntroSound,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        Icon = RoleIcons.Crusader
    };
}