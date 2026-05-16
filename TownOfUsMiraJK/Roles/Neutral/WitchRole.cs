using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.Events;
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
using System.Collections;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Events;
using TownOfUs.Events.TouEvents;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Roles.Other;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class WitchRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
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

    public bool HasWon { get; set; }
    public PlayerControl MarkedPlayer { get; set; }
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<SeerRole>());
    public DoomableType DoomHintType => DoomableType.Insight;
    public string LocaleKey => "Witch";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Mark", "Mark"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}MarkWikiDescription"),
                    NeutAssets.WitchMarkSprite),
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Control", "Control"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}ControlWikiDescription"),
                    NeutAssets.WitchControlSprite)
            };
        }
    }

    public Color RoleColor => Colors.Witch;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralEvil;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.ShapeshifterIntroSound,
        Icon = RoleIcons.Witch,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        TouRoleUtils.ClearTaskHeader(Player);
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
        return (gameOverReason != GameOverReason.CrewmatesByVote && gameOverReason != GameOverReason.CrewmatesByTask && gameOverReason != GameOverReason.CrewmateDisconnect && !Player.HasDied()) || HasWon;
    }

    [MethodRpc((uint)TownOfUsJKRpc.WitchControl, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcWitchControl(PlayerControl witch, PlayerControl target)
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.WitchControl, witch, target);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        if (target.AmOwner)
        {
            ButtonUsed buttonUsed = ButtonUsed.None;
            var killButton = HudManager.Instance.KillButton;
            var primaryButton = CustomButtonManager.Buttons.Where(x => x.Keybind == Keybinds.PrimaryAction && x.Button?.isActiveAndEnabled == true && x.CanClick()).FirstOrDefault();
            var secondaryButton = CustomButtonManager.Buttons.Where(x => x.Keybind == Keybinds.SecondaryAction && x.Button?.isActiveAndEnabled == true && x.CanClick()).FirstOrDefault();
            var tertiaryButton = CustomButtonManager.Buttons.Where(x => x.Keybind == Keybinds.TertiaryAction && x.Button?.isActiveAndEnabled == true && x.CanClick()).FirstOrDefault();
            if (killButton.CanInteract() && PlayerControl.LocalPlayer.killTimer <= 0 && killButton.isActiveAndEnabled)
            {
                killButton.DoClick();
                buttonUsed = ButtonUsed.Primary;
            }
            else if (primaryButton != null)
            {
                primaryButton.ClickHandler();
                buttonUsed = ButtonUsed.Primary;
            }
            else if (secondaryButton != null)
            {
                secondaryButton.ClickHandler();
                buttonUsed = ButtonUsed.Secondary;
            }
            else if (tertiaryButton != null)
            {
                tertiaryButton.ClickHandler();
                buttonUsed = ButtonUsed.Tertiary;
            }
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleWitchControlTargetNotif" + buttonUsed.ToString()).Replace("<role>", $"{Colors.Witch}{CustomRoleUtils.GetRegisteredRole((RoleTypes)RoleId.Get<WitchRole>())?.GetRoleName()}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Witch.LoadAsset());

            notif1.AdjustNotification();
            RpcWitchFeedback(target, witch, (byte)buttonUsed);
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.WitchFeedback, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcWitchFeedback(PlayerControl target, PlayerControl witch, byte buttonUsed)
    {
        ButtonUsed _buttonUsed = (ButtonUsed)buttonUsed;
        if (witch.AmOwner)
        {
            if (OptionGroupSingleton<WitchOptions>.Instance.Learns == WitchLearns.Nothing)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRoleWitchControlOwnerNotifNoRole" + _buttonUsed.ToString()).Replace("<player>", $"{Colors.Witch.ToTextColor()}{target.Data.PlayerName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Witch.LoadAsset());

                notif1.AdjustNotification();
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRoleWitchControlOwnerNotif" + buttonUsed.ToString()).Replace("<player>", $"{Colors.Witch.ToTextColor()}{target.Data.PlayerName}</color>").Replace("<role>", WitchResult(target)),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Witch.LoadAsset());

                notif1.AdjustNotification();
            }
        }
    }
    public static string WitchResult(PlayerControl player)
    {
        switch (OptionGroupSingleton<WitchOptions>.Instance.Learns)
        {
            case WitchLearns.Nothing:
                return "Unknown";
            case WitchLearns.Faction:
                if (player.IsCrewmate())
                {
                    return $"{Palette.CrewmateBlue.ToTextColor()}{TouLocale.Get("CrewmateKeyword")}</color>";
                }
                else if(player.IsImpostor())
                {
                    return $"{TownOfUsColors.Impostor.ToTextColor()}{TouLocale.Get("ImpostorKeyword")}</color>";
                }
                else
                {
                    return $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.Get("NeutralKeyword")}</color>";
                }
            case WitchLearns.Alignment:
                if (player.IsCrewmate())
                {
                    return $"{Palette.CrewmateBlue.ToTextColor()}{TouLocale.Get(player.Data.Role.GetRoleAlignment().ToString())}</color>";
                }
                else if (player.IsImpostor())
                {
                    return $"{TownOfUsColors.Impostor.ToTextColor()}{TouLocale.Get(player.Data.Role.GetRoleAlignment().ToString())}</color>";
                }
                else
                {
                    return $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.Get(player.Data.Role.GetRoleAlignment().ToString())}</color>";
                }
            case WitchLearns.Role:
                return $"{player.Data.Role.NameColor.ToTextColor()}{player.Data.Role.GetRoleName()}</color>";
            default:
                return "<color=red>Error</color>";
        }
    }
}
public enum ButtonUsed
{
    None,
    Primary,
    Secondary,
    Tertiary,
}