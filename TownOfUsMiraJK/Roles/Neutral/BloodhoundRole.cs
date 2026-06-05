using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class BloodhoundRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.GetParsed("NeutralKillingTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    public int KillCount { get; set; }
    public DoomableType DoomHintType => DoomableType.Hunter;
    public string LocaleKey => "Bloodhound";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => Colors.Bloodhound;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<BloodhoundOptions>.Instance.CanVent,
        IntroSound = TouAudio.WarlockIntroSound,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        Icon = RoleIcons.Bloodhound,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool HasImpostorVision => true;

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfUsRole.SetNewTabText(this);
        if (Player.HasModifier<BloodhoundBloodlustModifier>(x => !x.IsDestroyed))
        {
            stringB.Append(TownOfUsPlugin.Culture, $"\n<b>{TouLocale.GetParsed("TouJKRoleBloodhoundTabBloodlustCounter").Replace("<time>", $"{(int)Player.GetModifier<BloodhoundBloodlustModifier>().TimeRemaining}")}</b>");
        }
        else
        {
            stringB.Append(TownOfUsPlugin.Culture, $"\n<b>{TouLocale.GetParsed("TouJKRoleBloodhoundTabKillCounter").Replace("<count>", $"{KillCount}").Replace("<max_count>", $"{(int)OptionGroupSingleton<BloodhoundOptions>.Instance.KillsToBloodlust}")}</b>");
        }

        return stringB;
    }

    public bool WinConditionMet()
    {
        var bhCount = CustomRoleUtils.GetActiveRolesOfType<BloodhoundRole>().Count(x => !x.Player.HasDied());

        if (MiscUtils.KillersAliveCount > bhCount)
        {
            return false;
        }

        return bhCount >= Helpers.GetAlivePlayers().Count - bhCount;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = NeutAssets.BloodhoundVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(Colors.Bloodhound);
        }
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

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
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

    [MethodRpc((uint)TownOfUsJKRpc.TriggerBloodhound, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcTriggerBloodlust(PlayerControl bloodhound)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(bloodhound);
            return;
        }
        if (!bloodhound.TryGetModifier<BloodhoundBloodlustModifier>(out var modifier, x => !x.IsDestroyed))
        {
            bloodhound.AddModifier<BloodhoundBloodlustModifier>();
        }
        else
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleBloodhoundBloodlustProlongNotif"),
                Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Bloodhound.LoadAsset());

            notif1.AdjustNotification();
            modifier.ResetTimer();
            modifier.StartTimer();
        }
        var role = bloodhound.Data.Role as BloodhoundRole;
        role.KillCount -= (int)OptionGroupSingleton<BloodhoundOptions>.Instance.KillsToBloodlust;
    }
}