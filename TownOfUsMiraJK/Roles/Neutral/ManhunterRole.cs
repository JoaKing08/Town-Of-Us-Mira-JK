using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Collections;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Events;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
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
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Modifiers.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class ManhunterRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IAssignableTargets
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (!playerControl.AmOwner)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.GetParsed("NeutralOutlierTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    public bool IsUnlovable => true;
    public bool ContinuesGame => !Player.HasDied() && OptionGroupSingleton<ManhunterOptions>.Instance.StallGame && !TargetsDead && Helpers.GetAlivePlayers().Count <= 3;

    [HideFromIl2Cpp] public List<PlayerControl> Targets { get; set; } = [];

    public bool TargetsDead { get; set; }
    public int Priority { get; set; } = 5;

    public void AssignTargets()
    {
        if (!OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment)
        {
            return;
        }

        var manhunter = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(x =>
                x.IsRole<ManhunterRole>() && !x.HasDied() &&
                !SpectatorRole.TrackedSpectators.Contains(x.Data.PlayerName));

        if (manhunter == null)
        {
            var textlognotfound = $"Manhunter not found.";
            MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Error, textlognotfound);

            return;
        }

        var players = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => x.Data.Role is not ManhunterRole && x.Data.Role is not SpectatorRole).ToList();
        players.Shuffle();
        players.Shuffle();
        players.Shuffle();

        if (players.Count > 0)
        {
            for (int i = 0; i < Math.Min(players.Count, OptionGroupSingleton<ManhunterOptions>.Instance.Targets); i++)
            {
                RpcAddManhunterTarget(manhunter, players[i]);
            }
        }
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<SheriffRole>());
    public DoomableType DoomHintType => DoomableType.Hunter;
    public string LocaleKey => "Manhunter";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription").Replace("<symbol>", Colors.Manhunter.ToTextColor() + "/</color>") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => Colors.Manhunter;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralOutlier;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.ToppatIntroSound,
        Icon = RoleIcons.Manhunter,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool MetWinCon => TargetsDead;

    public bool WinConditionMet()
    {
        if (Player.HasDied())
        {
            return false;
        }

        if (!OptionGroupSingleton<ManhunterOptions>.Instance.StallGame)
        {
            return false;
        }

        if (!TargetsDead)
        {
            return false;
        }

        var result = Helpers.GetAlivePlayers().Contains(Player) && Helpers.GetAlivePlayers().Count <= 2 &&
                     MiscUtils.KillersAliveCount == 1;
        return result;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);
        stringB.AppendLine(TownOfUsPlugin.Culture, $"<b>{TouLocale.Get("TouJKRoleManhunterTabAddition")}</b>");
        foreach (var player in Targets.OrderBy(x => x.Data.PlayerName))
        {
            var mod = player.GetModifier<ManhunterTargetModifier>();
            var newText = $"<b><size=80%>{player.Data.PlayerName}</size></b>";
            if (player.HasDied())
            {
                var color = Color.grey;
                if (mod?.KilledByManhunter == false)
                {
                    color = Color.red;
                }
                else if (mod?.NewTarget == true)
                {
                    color = Color.Lerp(Color.cyan, Color.grey, 0.5f);
                }
                newText = $"<color=#{color.ToHtmlStringRGBA()}><s>{newText}</s></color>";
            }
            else if (player.GetModifier<ManhunterTargetModifier>()?.NewTarget == true)
            {
                newText = $"<color=#{Color.cyan.ToHtmlStringRGBA()}>{newText}</color>";
            }
            stringB.AppendLine(TownOfUsPlugin.Culture, $"{newText}");
        }

        return stringB;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Targets.Count == 0)
        {
            Targets = ModifierUtils.GetPlayersWithModifier<ManhunterTargetModifier>().Where(x => x != player)
                .ToList();
        }

        if (TutorialManager.InstanceExists && Targets.Count == 0 && Player.AmOwner && Player.IsHost() &&
            AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
        {
            Coroutines.Start(SetTutorialTargets(this));
        }
    }

    private static IEnumerator SetTutorialTargets(ManhunterRole manhunter)
    {
        yield return new WaitForSeconds(0.01f);
        manhunter.AssignTargets();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        TouRoleUtils.ClearTaskHeader(Player);
        if (TutorialManager.InstanceExists && Player.AmOwner)
        {
            var players = ModifierUtils.GetPlayersWithModifier<ManhunterTargetModifier>().ToList();
            players.Do(x => x.RpcRemoveModifier<ManhunterTargetModifier>());
        }

        if (!Player.HasModifier<BasicGhostModifier>() && TargetsDead)
        {
            Player.AddModifier<BasicGhostModifier>();
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
        return TargetsDead || WinConditionMet();
    }

    public void CheckTargetDeath(PlayerControl killed, PlayerControl? killer = null)
    {
        var mod = killed.GetModifier<ManhunterTargetModifier>();
        if (mod != null && mod.KilledByManhunter && (killer == null || killer.PlayerId != Player.PlayerId))
        {
            PlayerControl nextTarget = null;
            if (killer != null && !killer.HasModifier<ManhunterTargetModifier>())
            {
                nextTarget = killer;
            }
            if (nextTarget == null)
            {
                var targets = Helpers.GetAlivePlayers().Where(x => x.PlayerId != killed.PlayerId && x.PlayerId != Player.PlayerId && !x.HasModifier<ManhunterTargetModifier>());
                targets.Shuffle();
                nextTarget = targets.FirstOrDefault();
            }
            if (nextTarget != null)
            {
                var newMod = nextTarget.AddModifier<ManhunterTargetModifier>();
                Targets.Add(nextTarget);
                newMod!.NewTarget = true;
            }
            mod.KilledByManhunter = false;
        }

        if (Targets.Count > 0 && !Player.HasDied() && Targets.All(x => x.HasDied() || x == killed))
        {
            ManhunterWin(Player);
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.AddManhunterTarget)]
    public static void RpcAddManhunterTarget(PlayerControl player, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        if (player.Data.Role is not ManhunterRole)
        {
            Error("RpcAddManhunterTarget - Invalid Manhunter");
            return;
        }

        if (target == null)
        {
            return;
        }

        var role = player.GetRole<ManhunterRole>();

        if (role == null)
        {
            return;
        }

        role.Targets.Add(target);
        target.AddModifier<ManhunterTargetModifier>();
    }

    [MethodRpc((uint)TownOfUsJKRpc.ManhunterWin)]
    public static void RpcManhunterWin(PlayerControl player)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        ManhunterWin(player);
    }

    public static void ManhunterWin(PlayerControl player)
    {
        if (player.Data.Role is not ManhunterRole)
        {
            Error("RpcManhunterWin - Invalid Manhunter");
            return;
        }

        var man = player.GetRole<ManhunterRole>();
        man!.TargetsDead = true;
    }
}