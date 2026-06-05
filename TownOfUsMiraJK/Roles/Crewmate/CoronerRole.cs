using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate.Coroner;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class CoronerRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Death;
    public string LocaleKey => "Coroner";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");
    public static List<Type> CoronerResults = ModifierManager.Modifiers.Select(x => x.GetType()).Where(x => typeof(CoronerResultBaseModifier).IsAssignableFrom(x) && x != typeof(CoronerResultBaseModifier)).ToList();

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Autopsy", "Autopsy"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}AutopsyWikiDescription"),
                    ToUJKCrewAssets.CoronerAutopsySprite)
            };
        }
    }

    public Color RoleColor => TownOfUsMiraJKColors.Coroner;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.DetectiveIntroSound,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        Icon = ToUJKRoleIcons.Coroner
    };
    public bool CanAutopsy(byte body)
    {
        return CoronerResults.Any(x => !Player.HasModifier<CoronerResultBaseModifier>(y => y.GetType() == x && y.Body?.ParentId == body));
    }
    public void PerformAutopsy(DeadBody body)
    {
        if (!CanAutopsy(body.ParentId))
        {
            return;
        }
        var possibleResults = CoronerResults.Where(x => !Player.HasModifier<CoronerResultBaseModifier>(y => y.GetType() == x && y.Body?.ParentId == body.ParentId)).ToList();
        possibleResults.Shuffle();
        var result = possibleResults[0];
        Player.RpcAddModifier(result, body);
    }
}