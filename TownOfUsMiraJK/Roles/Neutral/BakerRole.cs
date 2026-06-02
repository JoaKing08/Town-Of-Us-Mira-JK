using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class BakerRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
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

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not BakerRole || Player.HasDied() || !Player.AmOwner)
        {
            return;
        }

        var allFed =
            ModifierUtils.GetPlayersWithModifier<BakerFedModifier>([HideFromIl2Cpp] (x) => !x.Player.HasDied() && x.BakerId == Player.PlayerId && !x.Player.IsApocalypse());
        var transformAnyway = allFed.Count() >= Helpers.GetAlivePlayers().Count(x => !x.IsApocalypse()) && OptionGroupSingleton<BakerOptions>.Instance.NotEnoughPlayersEffect == NotEnoughPlayersEffect.Transforms;

        if ((allFed.Count() >= OptionGroupSingleton<BakerOptions>.Instance.BreadToFamine &&
            (!MeetingHud.Instance || Helpers.GetAlivePlayers().Count > 2)) || transformAnyway)
        {
            FamineRole.RpcTriggerFamine(PlayerControl.LocalPlayer);
            CustomButtonSingleton<FamineStarveButton>.Instance.SetTimer(OptionGroupSingleton<BakerOptions>
                .Instance.StarveCooldown);
        }
        if (Helpers.GetAlivePlayers().Count(x => !x.IsApocalypse()) < OptionGroupSingleton<BakerOptions>.Instance.BreadToFamine && OptionGroupSingleton<BakerOptions>.Instance.NotEnoughPlayersEffect == NotEnoughPlayersEffect.Dies)
        {
            PlayerControl.LocalPlayer.RpcSpecialMurder(Player, MeetingCheck.Ignore,
                isIndirect: true,
                ignoreShield: true,
                resetKillTimer: false,
                causeOfDeath: "Famine");
        }
    }
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ClericRole>());
    public DoomableType DoomHintType => DoomableType.Protective;
    public string LocaleKey => "Baker";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Bread", "Bread"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}BreadWikiDescription"),
                    NeutAssets.BakerBreadSprite),
            };
        }
    }

    public Color RoleColor => Colors.Baker;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralOutlier;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.ChefSound,
        Icon = RoleIcons.Baker,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);

        var allFed = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && !x.IsApocalypse() &&
            x.GetModifier<BakerFedModifier>()?.BakerId == Player.PlayerId);

        if (allFed.HasAny())
        {
            stringB.Append(TownOfUsPlugin.Culture, $"\n<b>{TouLocale.Get("TouJKRoleBakerTabFedInfo")}</b>");
            foreach (var plr in allFed)
            {
                stringB.Append(TownOfUsPlugin.Culture, $"\n{Color.white.ToTextColor()}{plr.Data.PlayerName}</color>");
            }
        }

        var breadLeft = Math.Min(OptionGroupSingleton<BakerOptions>.Instance.BreadToFamine, Helpers.GetAlivePlayers().Count(x => !x.IsApocalypse())) - allFed.Count();

        stringB.Append(TownOfUsPlugin.Culture, $"\n\n<b>{TouLocale.GetParsed("TouJKRoleBakerTabBreadCounter").Replace("<count>", $"{breadLeft}")}</b>");

        return stringB;
    }

    public bool WinConditionMet()
    {
        return ApocalypseUtils.ApocalypseWinConditionMet();
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

    public static void GiveBread(PlayerControl source, PlayerControl target)
    {
        target.AddModifier<BakerFedModifier>(source.PlayerId);
    }

    [MethodRpc((uint)TownOfUsJKRpc.GiveBread)]
    public static void RpcGiveBread(PlayerControl source, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(source);
            return;
        }
        GiveBread(source, target);
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