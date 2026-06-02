using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using Reactor.Utilities;
using System.Collections;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class BerserkerRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.GetParsed("TouNeutralKillingTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }
    public int KillCount { get; set; }
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<HunterRole>());
    public DoomableType DoomHintType => DoomableType.Relentless;
    public string LocaleKey => "Berserker";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => Colors.Berserker;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.WarlockIntroSound,
        Icon = RoleIcons.Berserker,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        CanUseVent = OptionGroupSingleton<BerserkerOptions>.Instance.CanVent,
    };
    public bool HasImpostorVision => true;

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);
        stringB.Append(TownOfUsPlugin.Culture, $"\n<b>{TouLocale.GetParsed("TouJKRoleBerserkerTabKillCounter").Replace("<count>", $"{KillCount}").Replace("<max_count>", $"{(int)OptionGroupSingleton<BerserkerOptions>.Instance.KillsToTransform}")}</b>");

        return stringB;
    }

    public bool WinConditionMet()
    {
        return ApocalypseUtils.ApocalypseWinConditionMet();
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = NeutAssets.BerserkerVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(Colors.Berserker);
        }
        if (Player.AmOwner && (int)OptionGroupSingleton<BerserkerOptions>.Instance.WarChance != 0)
        {
            Coroutines.Start(CheckForWarChance(Player));
        }
    }
    private static IEnumerator CheckForWarChance(PlayerControl player)
    {
        yield return new WaitForSeconds(0.01f);

        System.Random rnd = new();
        var chance = rnd.Next(1, 101);

        if (chance <= OptionGroupSingleton<BerserkerOptions>.Instance.WarChance)
        {
            player.RpcChangeRole(RoleId.Get<WarRole>());
            CustomButtonSingleton<WarKillButton>.Instance.SetTimer(OptionGroupSingleton<BerserkerOptions>
                .Instance.WarKillCooldown);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TouAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfUsColors.Impostor);

            var button = CustomButtonSingleton<BakerBreadButton>.Instance;
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