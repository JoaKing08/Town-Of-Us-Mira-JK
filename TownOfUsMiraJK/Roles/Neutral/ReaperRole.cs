using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using System.Collections;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class ReaperRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
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

    public static List<byte> ReapedBodies { get; set; } = new();
    public int SoulCount { get; set; }
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ForensicRole>());
    public DoomableType DoomHintType => DoomableType.Death;
    public string LocaleKey => "Reaper";
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
                    ToUJKNeutAssets.ReaperReapSprite),
            };
        }
    }

    public Color RoleColor => TownOfUsMiraJKColors.Reaper;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralOutlier;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.MediumIntroSound,
        Icon = ToUJKRoleIcons.Reaper,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);
        stringB.Append(TownOfUsPlugin.Culture, $"\n<b>{TouLocale.GetParsed("TouJKRoleReaperTabSoulCounter").Replace("<count>", $"{SoulCount}").Replace("<max_count>", $"{(int)OptionGroupSingleton<ReaperOptions>.Instance.SoulsToTransform}")}</b>");

        return stringB;
    }

    public bool WinConditionMet()
    {
        return ApocalypseUtils.ApocalypseWinConditionMet(this);
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

    [MethodRpc((uint)TownOfUsJKRpc.CollectSoul)]
    public static void RpcReapSoul(PlayerControl soulCollector, DeadBody body)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(soulCollector);
            return;
        }
        var role = soulCollector.GetRole<ReaperRole>();
        ReapedBodies.Add(body.ParentId);
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

        if (!OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam)
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