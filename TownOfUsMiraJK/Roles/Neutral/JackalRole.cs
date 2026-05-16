using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using System.Collections;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Game.Neutral;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Roles.Other;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class JackalRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant, IUnlovable, IAssignableTargets, IContinuesGame
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.GetParsed("NeutralOutlierTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<MonarchRole>());
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string LocaleKey => "Jackal";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => UnlockedKill ? TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescriptionRecDead") : TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");
    public bool IsUnlovable => true;
    public int Priority { get; set; } = -10;
    public bool UnlockedKill { get; set; }
    public bool ContinuesGame => (!Player.HasDied() || ModifierUtils.GetPlayersWithModifier<JackalRecruitModifier>(x => !x.Player.HasDied()).Any()) && !WinConditionMet();

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => Colors.Jackal;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralOutlier;



    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<JackalOptions>.Instance.CanVent,
        IntroSound = TouAudio.ToppatIntroSound,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        Icon = RoleIcons.Jackal,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        MaxRoleCount = 1
    };

    public bool SetupIntroTeam(IntroCutscene instance,
        ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        if (Player != PlayerControl.LocalPlayer)
        {
            return true;
        }

        var jackTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

        jackTeam.Add(PlayerControl.LocalPlayer);
        foreach (var player in ModifierUtils.GetPlayersWithModifier<JackalRecruitModifier>())
        {
            jackTeam.Add(player);
        }

        yourTeam = jackTeam;

        return true;
    }

    public bool WinConditionMet()
    {
        var recruitCount = ModifierUtils.GetActiveModifiers<JackalRecruitModifier>().Count(x => !x.Player.HasDied()) + (Player.HasDied() ? 0 : 1);

        if (Helpers.GetAlivePlayers().Any(x => (x.IsImpostor() ||
        x.Is(RoleAlignment.NeutralKilling) ||
        (x.Data.Role is ITouCrewRole { IsPowerCrew: true } &&
         !(x.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.CrewContinuesGame) &&
         OptionGroupSingleton<GameMechanicOptions>.Instance.CrewKillersContinue)) &&
         !x.HasModifier<JackalRecruitModifier>()))
        {
            return false;
        }

        return recruitCount >= Helpers.GetAlivePlayers().Count - recruitCount;
    }
    public void AssignTargets()
    {
        if (!OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment)
        {
            return;
        }
        if (CustomRoleUtils.GetActiveRolesOfType<JackalRole>().Any() && !ModifierUtils.GetActiveModifiers<JackalRecruitModifier>().Any())
        {
            Dictionary<RoleTypes, List<PlayerControl>> factions = new();

            var crew = Helpers.GetAlivePlayers().Where(x => x.IsCrewmate() && !x.HasModifier<AllianceGameModifier>()).ToList();
            if (crew.Count != 0)
            {
                factions.Add(RoleTypes.Crewmate, crew);
            }

            var imp = Helpers.GetAlivePlayers().Where(x => x.IsImpostor() && !x.HasModifier<AllianceGameModifier>()).ToList();
            if (imp.Count != 0)
            {
                factions.Add(RoleTypes.Impostor, imp);
            }

            var apoc = Helpers.GetAlivePlayers().Where(x => x.IsApocalypse() && !x.HasModifier<AllianceGameModifier>()).ToList();
            if (apoc.Count != 0)
            {
                factions.Add(RoleTypes.Scientist, apoc);
            }

            foreach (var role in CustomRoleUtils.GetActiveRoles().Where(x => x.GetRoleAlignment() == RoleAlignment.NeutralKilling && x.Role != (RoleTypes)RoleId.Get<VampireRole>()))
            {
                if (!factions.ContainsKey(role.Role))
                {
                    var neut = Helpers.GetAlivePlayers().Where(x => x.Is(role.Role) && !x.HasModifier<AllianceGameModifier>()).ToList();
                    if (neut.Count != 0)
                    {
                        factions.Add(role.Role, neut);
                    }
                }
            }
            for (int i = 0; i < 2; i++)
            {
                var availablePlayers = factions.SelectMany(x => x.Value).ToList();
                availablePlayers.Shuffle();
                var recruit = availablePlayers.FirstOrDefault();
                if (recruit != null)
                {
                    recruit.RpcAddModifier<JackalRecruitModifier>();
                    if (recruit.IsCrewmate())
                    {
                        factions.Remove(RoleTypes.Crewmate);
                    }
                    else if (recruit.IsImpostor())
                    {
                        factions.Remove(RoleTypes.Impostor);
                    }
                    else if (recruit.IsApocalypse())
                    {
                        factions.Remove(RoleTypes.Scientist);
                    }
                    else
                    {
                        factions.Remove(recruit.Data.Role.Role);
                    }
                }
            }
        }
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = NeutAssets.JackalVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(Colors.Jackal);
        }

        if (!Player.HasModifier<BasicGhostModifier>())
        {
            Player.AddModifier<BasicGhostModifier>();
        }

        if (TutorialManager.InstanceExists && !PlayerControl.AllPlayerControls.ToArray().Any(x => x?.HasModifier<JackalRecruitModifier>() == true) &&
            AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started && PlayerControl.LocalPlayer.IsHost())
        {
            Coroutines.Start(SetTutorialTargets(this));
        }
    }

    private static IEnumerator SetTutorialTargets(JackalRole jack)
    {
        yield return new WaitForSeconds(0.01f);
        jack.AssignTargets();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        TouRoleUtils.ClearTaskHeader(Player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TouAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfUsColors.Impostor);
        }
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>()!;
        return console == null || console.AllowImpostor;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }
}