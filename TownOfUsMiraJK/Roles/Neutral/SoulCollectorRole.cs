using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using Innersloth.Assets;
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
using Reactor.Utilities.Extensions;
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
using TownOfUs.Options;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Patches.Options;
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

public sealed class SoulCollectorJKRole(IntPtr cppPtr)
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

    public List<byte> ReapedBodies = new List<byte>();
    public int SoulCount { get; set; }
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ForensicRole>());
    public DoomableType DoomHintType => DoomableType.Death;
    public string LocaleKey => "SoulCollector";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Reap", "Reap"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}ReapWikiDescription"),
                    NeutAssets.SoulCollectorReapSprite),
            };
        }
    }

    public Color RoleColor => Colors.SoulCollector;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => (RoleAlignment)27;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.MediumIntroSound,
        Icon = RoleIcons.SoulCollector,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);
        stringB.Append(TownOfUsPlugin.Culture, $"\n<b>{TouLocale.GetParsed("TouJKRoleSoulCollectorTabSoulCounter").Replace("<count>", $"{SoulCount}").Replace("<max_count>", $"{(int)OptionGroupSingleton<SoulCollectorJKOptions>.Instance.SoulsToTransform}")}</b>");

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

    RoleOptionsGroup ICustomRole.RoleOptionsGroup => CustomAlignmentsData.NeutralApocalypse;
    [MethodRpc((uint)TownOfUsJKRpc.CollectSoul)]
    public static void RpcReapSoul(PlayerControl soulCollector, DeadBody body)
    {
        var role = soulCollector.GetRole<SoulCollectorJKRole>();
        role.ReapedBodies.Add(body.ParentId);
        role.SoulCount++;
        Coroutines.Start(CoReapSoul(body));
    }
    public static IEnumerator CoReapSoul(DeadBody body)
    {
        var renderer = body.bodyRenderers[^1];
        yield return MiscUtils.PerformTimedAction(1f, t => renderer.color = Color.Lerp(Color.white, Color.gray, t));
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