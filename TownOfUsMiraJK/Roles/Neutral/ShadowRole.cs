using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class ShadowRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable
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

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouRole{LocaleKey}Vanish", "Vanish"),
                    TouLocale.GetParsed($"TouRole{LocaleKey}VanishWikiDescription"),
                    NeutAssets.ShadowVanishSprite),
                new(TouLocale.GetParsed($"TouRole{LocaleKey}Darkness", "Darkness"),
                    TouLocale.GetParsed($"TouRole{LocaleKey}DarknessWikiDescription"),
                    NeutAssets.ShadowDarknessSprite)
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
        MaxRoleCount = 1,
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
            HudManager.Instance.ImpostorVentButton.graphic.sprite = NeutAssets.ShadowVentSprite.LoadAsset();
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
}