using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class SanctifierRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public override bool IsAffectedByComms => false;
    public DoomableType DoomHintType => DoomableType.Protective;
    public string LocaleKey => "Sanctifier";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Sanctify", "Sanctify"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}SanctifyWikiDescription"),
                    ToUJKCrewAssets.SanctifierSanctifySprite)
            };
        }
    }

    public Color RoleColor => TownOfUsMiraJKColors.Sanctifier;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.TimeLordIntroSound,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        Icon = ToUJKRoleIcons.Sanctifier
    };

    [MethodRpc((uint)TownOfUsJKRpc.SanctifierSanctify)]
    public static void RpcSanctifierSanctify(PlayerControl sanctifier, Vector2 position, float z)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(sanctifier);
            return;
        }
        var v3pos = (Vector3)position;
        v3pos.z = z;
        SanctifierCircle.Create(v3pos, scale: OptionGroupSingleton<SanctifierOptions>.Instance.SanctifySize * ShipStatus.Instance.MaxLightRadius * 2f);
    }
}