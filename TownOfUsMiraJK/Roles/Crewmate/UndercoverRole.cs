using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using System.Collections;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
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
    public bool CoverCanVent => Player?.GetModifier<UndercoverCoverModifier>()?.ShownRole is not ICustomRole role || role?.Configuration.CanUseVent != false;

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
        CanUseVent = OptionGroupSingleton<UndercoverOptions>.Instance.CanVent && CoverCanVent,
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
        var notInPlay = (RoleBehaviour x) => CustomRoleUtils.GetActiveRoles().Count(y => x.GetType() == y.GetType()) < GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(x.Role);

        foreach (var unc in uncs)
        {
            var covers = new List<(ushort RoleType, int Chance)>();
            if (OptionGroupSingleton<UndercoverOptions>.Instance.CanBeConcealing)
            {
                covers.AddRange(MiscUtils.GetRolesToAssign(RoleAlignment.ImpostorConcealing, notInPlay));
            }
            if (OptionGroupSingleton<UndercoverOptions>.Instance.CanBeKilling)
            {
                covers.AddRange(MiscUtils.GetRolesToAssign(RoleAlignment.ImpostorKilling, notInPlay));
            }
            if (OptionGroupSingleton<UndercoverOptions>.Instance.CanBePower)
            {
                covers.AddRange(MiscUtils.GetRolesToAssign(RoleAlignment.ImpostorPower, notInPlay));
            }
            if (OptionGroupSingleton<UndercoverOptions>.Instance.CanBeSupport)
            {
                covers.AddRange(MiscUtils.GetRolesToAssign(RoleAlignment.ImpostorSupport, notInPlay));
            }
            ushort cover = covers.Any() ? MiscUtils.GetMaxRolesToAssign(ModdedRoleTeams.Impostor, 1, (x) => covers.Any(y => y.RoleType == (ushort)x.Role)).FirstOrDefault() : (ushort)RoleTypes.Impostor;
            if (cover == 0)
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