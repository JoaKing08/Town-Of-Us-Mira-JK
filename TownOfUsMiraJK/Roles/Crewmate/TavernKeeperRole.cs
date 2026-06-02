using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class TavernKeeperRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string LocaleKey => "TavernKeeper";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Drink", "Drink"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}DrinkWikiDescription"),
                    CrewAssets.TavernKeeperDrinkSprite)
            };
        }
    }

    public Color RoleColor => Colors.TavernKeeper;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = RoleIcons.TavernKeeper,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        IntroSound = TouAudio.CrewmateIntroSound
    };
}