using AmongUs.GameOptions;
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
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Options;
using TownOfUs.Options.Modifiers.Alliance;
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

public sealed class FamineRole(IntPtr cppPtr)
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

    public float AnnounceIn { get; set; }
    public bool Announced { get; set; }
    public float PassiveStarvation { get; set; } = OptionGroupSingleton<BakerOptions>.Instance.PassiveStarvingCooldown;
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<SeerRole>());
    public DoomableType DoomHintType => DoomableType.Protective;
    public string YouAreText => TouLocale.Get("YouAre");
    public string YouWereText => TouLocale.Get("YouWere");
    public string LocaleKey => "Famine";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}PassiveStarve", "Passive Starvation"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}PassiveStarveWikiDescription"),
                    NeutAssets.FamineStarveSprite),
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Starve", "Starve"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}StarveWikiDescription"),
                    NeutAssets.FamineStarveSprite),
            };
        }
    }

    public Color RoleColor => Colors.Famine;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralOutlier;
    public bool HasImpostorVision => true;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<BakerOptions>.Instance.CanVent,
        HideSettings = true,
        CanModifyChance = false,
        DefaultChance = 0,
        DefaultRoleCount = 0,
        MaxRoleCount = 0,
        IntroSound = TouAudio.ChefSound,
        Icon = RoleIcons.Famine,
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
    public RoleBehaviour AppearAs => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<BakerRole>());

    [MethodRpc((uint)TownOfUsJKRpc.TriggerFamine, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcTriggerFamine(PlayerControl player)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        if (player.HasDied() || (player.Data.Role is not FamineRole && player.Data.Role is not BakerRole))
        {
            return;
        }
        if (player.Data.Role is not FamineRole)
        {
            player.ChangeRole(RoleId.Get<FamineRole>());
        }
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (!Player.HasModifier<InvulnerabilityModifier>())
        {
            Player.AddModifier<InvulnerabilityModifier>(false, false, false);
        }

        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = NeutAssets.FamineVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(Colors.Famine);
        }

        PassiveStarvation = OptionGroupSingleton<BakerOptions>.Instance.PassiveStarvingCooldown;
        AnnounceIn = OptionGroupSingleton<BakerOptions>.Instance.AnnounceTransformationDelay;
        Announced = false;
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        TouRoleUtils.ClearTaskHeader(Player);
        if (Player.HasModifier<InvulnerabilityModifier>())
        {
            Player.RemoveModifier<InvulnerabilityModifier>();
        }

        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TouAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfUsColors.Impostor);
        }
    }

    
    public void Update()
    {
        if (Player == null)
        {
            return;
        }
        if (!Announced)
        {
            AnnounceIn -= Time.unscaledDeltaTime;
            if (AnnounceIn <= 0f)
            {
                Announced = true;
                var title = $"<color=#{Colors.Baker.ToHtmlStringRGBA()}>{TouLocale.Get("TouJKRoleFamineMessageTitle")}</color>";
                var msg = TouLocale.GetParsed("TouJKRoleFamineAnnounceMessage");

                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{msg.Replace("<role>", $"{Colors.Famine.ToTextColor()}{RoleName}</color>")}</b>", Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Famine.LoadAsset());

                notif1.AdjustNotification();

                MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg.Replace("<role>", MiscUtils.GetHyperlinkText(this)), false, true);
            }
        }
        if (MeetingHud.Instance == null && ExileController.Instance == null && Player.AmOwner)
        {
            PassiveStarvation -= Time.unscaledDeltaTime;
            if (PassiveStarvation <= 0f)
            {
                RpcDoPassiveStarve(Player);
                PassiveStarvation = OptionGroupSingleton<BakerOptions>.Instance.PassiveStarvingCooldown;
            }
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
    public void DoPassiveStarve()
    {
        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed((PlayerControl.LocalPlayer.IsApocalypse() && !(Player.IsLover() && OptionGroupSingleton<LoversOptions>.Instance.LoverKillTeammates) && OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam) || (Player.IsLover() && PlayerControl.LocalPlayer.IsLover() && !OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther) ? "TouJKRoleFamineStarveMessageApoc" : "TouJKRoleFamineStarveMessage").Replace("<role>", $"{Colors.Famine.ToTextColor()}{RoleName}</color>"), Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Famine.LoadAsset());

        notif1.AdjustNotification();
        foreach (var player in Helpers.GetAlivePlayers().Where(x => (!x.IsApocalypseAligned() || (Player.IsLover() && OptionGroupSingleton<LoversOptions>.Instance.LoverKillTeammates) || !OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam) && (!Player.IsLover() || !x.IsLover() || OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther)))
        {
            if (!player.TryGetModifier<BakerFedModifier>(out var modifier))
            {
                if (Player.AmOwner)
                {
                    PlayerControl.LocalPlayer.RpcSpecialMurder(player, MeetingCheck.Ignore,
                    teleportMurderer: false,
                    isIndirect: true,
                    ignoreShield: true,
                    resetKillTimer: false,
                    causeOfDeath: "Famine");
                }
            }
            else
            {
                modifier.BreadLeft -= 1;
                if (modifier.BreadLeft <= 0)
                {
                    player.RemoveModifier(modifier);
                }
            }
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.PassiveStarve)]
    public static void RpcDoPassiveStarve(PlayerControl source)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(source);
            return;
        }
        ((FamineRole)source.Data.Role).DoPassiveStarve();
    }

    public bool WinConditionMet()
    {
        return ApocalypseUtils.ApocalypseWinConditionMet(this);
    }

    public void OffsetButtons()
    {
        var canVent = OptionGroupSingleton<BakerOptions>.Instance.CanVent || LocalSettingsTabSingleton<TownOfUsLocalSettings>.Instance.OffsetButtonsToggle.Value;
        var starve = CustomButtonSingleton<FamineStarveButton>.Instance;
        Coroutines.Start(MiscUtils.CoMoveButtonIndex(starve, !canVent));
    }

    public static void Starve(PlayerControl source, PlayerControl target)
    {
        for (int i = 0; i < OptionGroupSingleton<BakerOptions>.Instance.StarveStrength; i++)
        {
            if (!target.TryGetModifier<BakerFedModifier>(out var modifier))
            {
                if (source.AmOwner)
                {
                    PlayerControl.LocalPlayer.RpcSpecialMurder(target, MeetingCheck.Ignore,
                    teleportMurderer: false,
                    isIndirect: true,
                    ignoreShield: true,
                    resetKillTimer: false,
                    causeOfDeath: "Famine");
                }
            }
            else
            {
                modifier.BreadLeft -= 1;
                if (modifier.BreadLeft <= 0)
                {
                    target.RemoveModifier(modifier);
                }
            }
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.Starve)]
    public static void RpcStarve(PlayerControl source, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(source);
            return;
        }
        Starve(source, target);
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