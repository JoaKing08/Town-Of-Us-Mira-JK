using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Impostor;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Impostor;

public sealed class SniperRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public DoomableType DoomHintType => DoomableType.Perception;
    public string LocaleKey => "Sniper";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<OfficerRole>());
    public Color RoleColor => TownOfUsColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        OptionsScreenshot = TouBanners.ImpostorRoleBanner,
        Icon = ToUJKRoleIcons.Sniper,
        UseVanillaKillButton = OptionGroupSingleton<SniperOptions>.Instance.CanKill
    };

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Aim", "Aim"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}AimWikiDescription"),
                    ToUJKImpAssets.SniperAimSprite),
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Shoot", "Shoot"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}ShootWikiDescription"),
                    ToUJKImpAssets.SniperShootSprite)
            };
        }
    }
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }

    [MethodRpc((uint)TownOfUsJKRpc.SniperShoot, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcSniperShoot(PlayerControl sniper, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(sniper);
            return;
        }
        var opts = OptionGroupSingleton<SniperOptions>.Instance;
        if (opts.AnnounceShot)
        {
            if (opts.ShowSniper)
            {
                PlayerControl.LocalPlayer.AddModifier<SniperArrowModifier>((Vector2)sniper.transform.position, TownOfUsColors.Impostor);
            }
            if (opts.ShowVictim)
            {
                PlayerControl.LocalPlayer.AddModifier<SniperArrowModifier>((Vector2)target.transform.position, TownOfUsColors.Impostor);
            }

            Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Impostor));

            if (sniper.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRoleSniperShootOwnerNotif").Replace("<player>", $"{TownOfUsColors.Impostor.ToTextColor()}{target.Data.PlayerName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Sniper.LoadAsset());

                notif1.AdjustNotification();
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRoleSniperShootNotif").Replace("<role>", $"{TownOfUsColors.Impostor.ToTextColor()}{sniper.Data.Role.GetRoleName()}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Sniper.LoadAsset());

                notif1.AdjustNotification();
            }
        }

        if (sniper.AmOwner)
        {
            sniper.RpcSpecialMurder(target, true, teleportMurderer: false, causeOfDeath: "Sniper");
        }
        target.RemoveModifier<SniperTargetModifier>(x => x.Owner.PlayerId == sniper.PlayerId);
    }
}