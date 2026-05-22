using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Options.Roles.Secret;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Secret;

public sealed class ShadowRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IUnguessable
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
    public bool IsHiddenFromList => MiscUtils.CurrentGamemode() is not TouGamemode.Normal || !PlayerControl.LocalPlayer.IsRole<ShadowRole>();
    public string LocaleKey => "Shadow";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");
    public bool IsGuessable => false;
    public RoleBehaviour AppearAs => RoleManager.Instance.GetRole(RoleTypes.Crewmate);

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouRole{LocaleKey}Vanish", "Vanish"),
                    TouLocale.GetParsed($"TouRole{LocaleKey}VanishWikiDescription"),
                    SecrAssets.ShadowVanishSprite),
                new(TouLocale.GetParsed($"TouRole{LocaleKey}Darkness", "Darkness"),
                    TouLocale.GetParsed($"TouRole{LocaleKey}DarknessWikiDescription"),
                    SecrAssets.ShadowDarknessSprite)
            };
        }
    }

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => Colors.Shadow;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<ShadowOptions>.Instance.CanVent,
        IntroSound = TouAudio.PhantomIntroSound,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        Icon = RoleIcons.Shadow,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        DefaultRoleCount = 1,
        MaxRoleCount = 1,
        DefaultChance = 100,
        HideSettings = true
    };

    public bool HasImpostorVision => true;

    public bool WinConditionMet()
    {
        var shCount = CustomRoleUtils.GetActiveRolesOfType<ShadowRole>().Count(x => !x.Player.HasDied());

        if (MiscUtils.KillersAliveCount > shCount)
        {
            return false;
        }

        return shCount >= Helpers.GetAlivePlayers().Count - shCount;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = SecrAssets.ShadowVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(Colors.Shadow);
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

    bool ICustomRole.CanSpawnOnCurrentMode() => !GameManager.Instance.IsHideAndSeek() && UnityEngine.Random.RandomRange(0, 100) <= OptionGroupSingleton<ShadowOptions>.Instance.ShadowChance;
}