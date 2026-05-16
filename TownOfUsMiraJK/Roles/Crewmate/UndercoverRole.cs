using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class UndercoverRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public static bool InPlay => CustomRoleUtils.GetActiveRoles().Any(x => x.Role == (RoleTypes)RoleId.Get<UndercoverRole>());
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string LocaleKey => "Undercover";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Cover", "Cover"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}CoverWikiDescription"),
                    RoleIcons.Undercover)
            };
        }
    }

    public Color RoleColor => Colors.Undercover;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = RoleIcons.Undercover,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        IntroSound = TouAudio.SpyIntroSound
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (PlayerControl.LocalPlayer.IsHost())
        {
            AssignCover();
        }
    }
    public void AssignCover()
    {
        var excluded = MiscUtils.SpawnableRoles.Where(x => x is ISpawnChange { NoSpawn: true }).Select(x => x.Role);
        var impConPred = (RoleBehaviour x) => !(x.GetRoleAlignment() == RoleAlignment.ImpostorConcealing && !OptionGroupSingleton<UndercoverOptions>.Instance.CanBeConcealing);
        var impKilPred = (RoleBehaviour x) => !(x.GetRoleAlignment() == RoleAlignment.ImpostorKilling && !OptionGroupSingleton<UndercoverOptions>.Instance.CanBeKilling);
        var impPowPred = (RoleBehaviour x) => !(x.GetRoleAlignment() == RoleAlignment.ImpostorPower && !OptionGroupSingleton<UndercoverOptions>.Instance.CanBePower);
        var impSupPred = (RoleBehaviour x) => !(x.GetRoleAlignment() == RoleAlignment.ImpostorSupport && !OptionGroupSingleton<UndercoverOptions>.Instance.CanBeSupport);

        var cover =
            MiscUtils.GetMaxRolesToAssign(ModdedRoleTeams.Impostor, 1, x => !excluded.Contains(x.Role) && impConPred(x) && impKilPred(x) && impPowPred(x) && impSupPred(x) && !CustomRoleUtils.GetActiveRoles().Any(y => x.Role == y.Role)).FirstOrDefault();
        if (cover == null)
        {
            cover = (ushort)RoleTypes.Impostor;
        }
        Player.RpcAddModifier<UndercoverCoverModifier>(cover);
    }
}