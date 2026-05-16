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
using Reactor.Utilities;
using System.Collections;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.GameOptions;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class PlaguebearerJKRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{Colors.Apocalypse.ToTextColor()}{TouLocale.GetParsed("TouJKNeutralApocalypseTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not PlaguebearerJKRole || Player.HasDied() || !Player.AmOwner)
        {
            return;
        }

        var allInfected =
            ModifierUtils.GetPlayersWithModifier<PlaguebearerInfectedModifier>([HideFromIl2Cpp] (x) => !x.Player.HasDied() && x.PlagueBearerId == Player.PlayerId && !x.Player.IsApocalypseAligned());

        if (allInfected.Count() >= Helpers.GetAlivePlayers().Count(x => !x.IsApocalypseAligned()) &&
            (!MeetingHud.Instance || Helpers.GetAlivePlayers().Count > 2))
        {
            PestilenceRole.RpcTriggerPestilence(PlayerControl.LocalPlayer);

            CustomButtonSingleton<PestilenceKillButton>.Instance.SetTimer(OptionGroupSingleton<PlaguebearerJKOptions>
                .Instance.PestKillCooldown);
        }
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<AurialRole>());
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string LocaleKey => "Plaguebearer";
    public string RoleName => TouLocale.Get($"TouRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouRole{LocaleKey}Infect", "Infect"),
                    TouLocale.GetParsed($"TouRole{LocaleKey}InfectWikiDescription"),
                    TouNeutAssets.InfectSprite)
            };
        }
    }

    public Color RoleColor => TownOfUsColors.Plaguebearer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => (RoleAlignment)27;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.PhantomIntroSound,
        Icon = TouRoleIcons.Plaguebearer,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);

        var allInfected = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && !x.IsApocalypseAligned() &&
            x.GetModifier<PlaguebearerInfectedModifier>()?.PlagueBearerId == Player.PlayerId);

        if (allInfected.HasAny())
        {
            stringB.Append(TownOfUsPlugin.Culture, $"\n<b>{TouLocale.Get("TouRolePlaguebearerTabInfectedInfo")}</b>");
            foreach (var plr in allInfected)
            {
                stringB.Append(TownOfUsPlugin.Culture, $"\n{Color.white.ToTextColor()}{plr.Data.PlayerName}</color>");
            }
        }

        var notInfected = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && !x.IsApocalypseAligned() && !x.HasModifier<PlaguebearerInfectedModifier>());

        stringB.Append(TownOfUsPlugin.Culture, $"\n\n<b>{TouLocale.GetParsed("TouRolePlaguebearerTabInfectCounter").Replace("<count>", $"{notInfected.Count()}")}</b>");

        return stringB;
    }

    public bool WinConditionMet()
    {
        return ApocalypseUtils.ApocalypseWinConditionMet();
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        Player.AddModifier<PlaguebearerInfectedModifier>(Player.PlayerId);
        if (Player.AmOwner && (int)OptionGroupSingleton<PlaguebearerJKOptions>.Instance.PestChance != 0)
        {
            Coroutines.Start(CheckForPestChance(Player));
        }
    }

    private static IEnumerator CheckForPestChance(PlayerControl player)
    {
        yield return new WaitForSeconds(0.01f);

        System.Random rnd = new();
        var chance = rnd.Next(1, 101);

        if (chance <= OptionGroupSingleton<PlaguebearerJKOptions>.Instance.PestChance)
        {
            player.RpcChangeRole(RoleId.Get<PestilenceRole>());
            CustomButtonSingleton<PestilenceKillButton>.Instance.SetTimer(OptionGroupSingleton<PlaguebearerJKOptions>
                .Instance.PestKillCooldown);
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

    public static void CheckInfected(PlayerControl source, PlayerControl target)
    {
        var sourceInfection = source.TryGetModifier<PlaguebearerInfectedModifier>(out var sourceMod) ? sourceMod : null;
        var targetInfection = target.TryGetModifier<PlaguebearerInfectedModifier>(out var targetMod) ? targetMod : null;
        var sourcePb = source.Data.Role is PlaguebearerJKRole;
        var targetPb = target.Data.Role is PlaguebearerJKRole;
        if (sourcePb && targetInfection == null)
        {
            target.AddModifier<PlaguebearerInfectedModifier>(source.PlayerId);
        }
        else if (targetPb && sourceInfection == null)
        {
            source.AddModifier<PlaguebearerInfectedModifier>(target.PlayerId);
        }
        else if (sourceInfection != null && targetInfection == null)
        {
            target.AddModifier<PlaguebearerInfectedModifier>(sourceInfection.PlagueBearerId);
        }
        else if (targetInfection != null && sourceInfection == null)
        {
            source.AddModifier<PlaguebearerInfectedModifier>(targetInfection.PlagueBearerId);
        }
    }

    public static void RpcCheckInfected(PlayerControl source, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(source);
            return;
        }
        CheckInfected(source, target);
    }
    RoleOptionsGroup ICustomRole.RoleOptionsGroup => CustomAlignmentsData.NeutralApocalypse;
    [HarmonyPatch]
    public static class DisableOriginal
    {
        [HarmonyPatch(typeof(PlaguebearerRole), nameof(PlaguebearerRole.Configuration), MethodType.Getter)]
        [HarmonyPostfix]
        public static void DisableOptions(ref CustomRoleConfiguration __result)
        {
            __result.HideSettings = true;
        }
        [HarmonyPatch(typeof(MiscUtils), nameof(MiscUtils.AllRegisteredRoles), MethodType.Getter)]
        [HarmonyPostfix]
        public static void RemoveFromAllRegisteredRoles(ref IEnumerable<RoleBehaviour> __result)
        {
            __result = __result.Where(x => x.Role != (RoleTypes)RoleId.Get<PlaguebearerRole>());
        }
        [HarmonyPatch(typeof(MiscUtils), nameof(MiscUtils.AllRoles), MethodType.Getter)]
        [HarmonyPostfix]
        public static void RemoveFromAllRoles(ref IEnumerable<RoleBehaviour> __result)
        {
            __result = __result.Where(x => x.Role != (RoleTypes)RoleId.Get<PlaguebearerRole>());
        }
        [HarmonyPatch(typeof(PlaguebearerRole), nameof(PlaguebearerRole.RpcCheckInfected))]
        [HarmonyPrefix]
        public static bool RedirectRpcCheckInfected(PlayerControl source, PlayerControl target)
        {
            RpcCheckInfected(source, target);
            return false;
        }
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
    TeamIntroConfiguration? IntroConfiguration => new(
        Colors.Apocalypse,
        TouLocale.Get("TouJKApocalypse"),
        TouLocale.Get("TouJKApocalypseDesc"));
}