using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Collections;
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
using TownOfUs.Options;
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
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class UndercoverRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, IAssignableTargets
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

        if (TutorialManager.InstanceExists && !Player.HasModifier<UndercoverCoverModifier>() &&
            AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started && PlayerControl.LocalPlayer.IsHost())
        {
            Coroutines.Start(SetTutorialTargets(this));
        }
    }
    public int Priority { get; set; } = 0;
    public void AssignTargets()
    {
        if (!OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment)
        {
            return;
        }

        var uncs = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => x.IsRole<DemagogueRole>() && !x.HasDied());

        var excluded = MiscUtils.SpawnableRoles.Where(x => x is ISpawnChange { NoSpawn: true }).Select(x => x.Role).ToList();
        var impConPred = (RoleBehaviour x) => !(x.GetRoleAlignment() == RoleAlignment.ImpostorConcealing && !OptionGroupSingleton<UndercoverOptions>.Instance.CanBeConcealing);
        var impKilPred = (RoleBehaviour x) => !(x.GetRoleAlignment() == RoleAlignment.ImpostorKilling && !OptionGroupSingleton<UndercoverOptions>.Instance.CanBeKilling);
        var impPowPred = (RoleBehaviour x) => !(x.GetRoleAlignment() == RoleAlignment.ImpostorPower && !OptionGroupSingleton<UndercoverOptions>.Instance.CanBePower);
        var impSupPred = (RoleBehaviour x) => !(x.GetRoleAlignment() == RoleAlignment.ImpostorSupport && !OptionGroupSingleton<UndercoverOptions>.Instance.CanBeSupport);

        foreach (var unc in uncs)
        {
            var cover =
                MiscUtils.GetMaxRolesToAssign(ModdedRoleTeams.Impostor, 1, x => !excluded.Contains(x.Role) && impConPred(x) && impKilPred(x) && impPowPred(x) && impSupPred(x) && !CustomRoleUtils.GetActiveRoles().Any(y => x.Role == y.Role) && !ModifierUtils.GetActiveModifiers<UndercoverCoverModifier>(y => x.Role == y.ShownRole?.Role).Any()).FirstOrDefault();
            if (cover == (ushort)RoleTypes.Crewmate)
            {
                cover = (ushort)RoleTypes.Impostor;
            }
            Player.RpcAddModifier<UndercoverCoverModifier>(cover);
            excluded.Add((RoleTypes)cover);
        }
    }
    private static IEnumerator SetTutorialTargets(UndercoverRole unc)
    {
        yield return new WaitForSeconds(0.01f);
        unc.AssignTargets();
    }
}