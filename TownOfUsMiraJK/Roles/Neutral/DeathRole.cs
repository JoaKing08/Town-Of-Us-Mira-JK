using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modules.Components;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class DeathRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, IUnguessable, ICrewVariant
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.GetParsed("TouNeutralOutlierTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<SnitchRole>());
    public DoomableType DoomHintType => DoomableType.Protective;
    public string YouAreText => TouLocale.Get("YouAre");
    public string YouWereText => TouLocale.Get("YouWere");
    public string LocaleKey => "Death";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Armageddon", "Armageddon"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}ArmageddonWikiDescription"),
                    RoleIcons.Death)
            };
        }
    }

    public Color RoleColor => Colors.Death;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralOutlier;

    public CustomRoleConfiguration Configuration => new(this)
    {
        HideSettings = true,
        CanModifyChance = false,
        DefaultChance = 0,
        DefaultRoleCount = 0,
        MaxRoleCount = 0,
        IntroSound = TouAudio.MediumIntroSound,
        Icon = RoleIcons.Death,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = new StringBuilder();
        stringB.AppendLine(TownOfUsPlugin.Culture,
            $"{RoleColor.ToTextColor()}{YouAreText}<b> {RoleName},‎ ‎ ‎ \n<size=80%>{RoleDescription}</size></b></color>");
        stringB.AppendLine(TownOfUsPlugin.Culture,
            $"<size=60%>{TouLocale.Get("Alignment")}: <b>{MiscUtils.GetParsedRoleAlignment(RoleAlignment, true)}</b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(TownOfUsPlugin.Culture, $"{RoleLongDescription}");

        return stringB;
    }

    public bool IsGuessable => false;
    public RoleBehaviour AppearAs => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ReaperJKRole>());

    [MethodRpc((uint)TownOfUsJKRpc.TriggerDeath, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcTriggerDeath(PlayerControl player)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        if (player.HasDied() || (player.Data.Role is not DeathRole && player.Data.Role is not ReaperJKRole))
        {
            return;
        }
        if (player.Data.Role is not DeathRole)
        {
            player.ChangeRole(RoleId.Get<DeathRole>());
        }
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (!Player.HasModifier<InvulnerabilityModifier>())
        {
            Player.AddModifier<InvulnerabilityModifier>(false, false, false);
        }

        var title = $"<color=#{Colors.Reaper.ToHtmlStringRGBA()}>{TouLocale.Get("TouJKRoleDeathMessageTitle")}</color>";
        var msg = TouLocale.GetParsed("TouJKRoleDeathAnnounceMessage");

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{msg.Replace("<role>", $"{Colors.Death.ToTextColor()}{RoleName}</color>")}</b>", Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Death.LoadAsset());

        notif1.AdjustNotification();

        MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg.Replace("<role>", MiscUtils.GetHyperlinkText(this)), false, true);

        ArmageddonSabotageSystem.ArmageddonFinished = false;

        if (!Player.AmOwner)
        {
            return;
        }

        if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
        {
            var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
            if (lifeSuppSystemType != null)
            {
                lifeSuppSystemType.Countdown = 10000f;
            }
        }

        foreach (var systemType2 in ShipStatus.Instance.Systems.Values)
        {
            var sabo = systemType2.TryCast<ICriticalSabotage>();
            if (sabo == null)
            {
                continue;
            }

            sabo.ClearSabotage();
        }

        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Sabotage, ArmageddonSabotageSystem.SabotageId);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        TouRoleUtils.ClearTaskHeader(Player);
        if (Player.HasModifier<InvulnerabilityModifier>())
        {
            Player.RemoveModifier<InvulnerabilityModifier>();
        }

        ArmageddonSabotageSystem.ArmageddonFinished = false;
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

    public bool WinConditionMet()
    {
        return ApocalypseUtils.ApocalypseWinConditionMet();
    }

    public bool SetupIntroTeam(IntroCutscene instance,
        ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        if (Player != PlayerControl.LocalPlayer)
        {
            return true;
        }

        var apocTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

        apocTeam.Add(PlayerControl.LocalPlayer);
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x?.IsApocalypseAligned() == true && !x.AmOwner))
        {
            apocTeam.Add(player);
        }

        yourTeam = apocTeam;

        return true;
    }
}