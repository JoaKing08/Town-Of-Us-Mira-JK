using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Impostor;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Impostor;

public sealed class GodfatherRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public bool Recruited { get; set; }
    public MafiosoRole? Mafioso => CustomRoleUtils.GetActiveRolesOfType<MafiosoRole>()?.FirstOrDefault(x => x.Godfather.PlayerId == Player.PlayerId);
    public bool MafiosoAlive => Mafioso != null && !Mafioso.Player.HasDied();
    public DoomableType DoomHintType => DoomableType.Protective;
    public string LocaleKey => "Godfather";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<PoliticianRole>());
    public Color RoleColor => TownOfUsColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorPower;

    public CustomRoleConfiguration Configuration => new(this)
    {
        OptionsScreenshot = TouBanners.ImpostorRoleBanner,
        Icon = ToUJKRoleIcons.Godfather,
        UseVanillaKillButton = OptionGroupSingleton<GodfatherOptions>.Instance.CanKill || (!MafiosoAlive && (Recruited || OptionGroupSingleton<GodfatherOptions>.Instance.CanKillBeforeMafioso))
    };

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Recruit", "Recruit"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}RecruitWikiDescription"),
                    ToUJKImpAssets.GodfatherRecruitSprite)
            };
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.GodfatherRecruit, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcGodfatherRecruit(PlayerControl godfather, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(godfather);
            return;
        }
        if (godfather.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleGodfatherRecruitOwnerNotif").Replace("<player>", target.Data.PlayerName).Replace("<role>", $"{TownOfUsColors.Impostor.ToTextColor()}{CustomRoleUtils.GetRegisteredRole((RoleTypes)RoleId.Get<MafiosoRole>())?.GetRoleName() ?? ""}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Godfather.LoadAsset());

            notif1.AdjustNotification();
        }
        else if (target.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleGodfatherRecruitTargetNotif").Replace("<player>", target.Data.PlayerName).Replace("<new_role>", $"{TownOfUsColors.Impostor.ToTextColor()}{CustomRoleUtils.GetRegisteredRole((RoleTypes)RoleId.Get<MafiosoRole>())?.GetRoleName() ?? ""}</color>").Replace("<role>", $"{TownOfUsColors.Impostor.ToTextColor()}{CustomRoleUtils.GetRegisteredRole((RoleTypes)RoleId.Get<GodfatherRole>())?.GetRoleName() ?? ""}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Mafioso.LoadAsset());

            notif1.AdjustNotification();
        }
        target.ChangeRole(RoleId.Get<MafiosoRole>());
        (target.Data.Role as MafiosoRole)!.Godfather = godfather;
    }
}