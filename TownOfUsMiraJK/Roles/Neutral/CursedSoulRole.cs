using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral;

public sealed class CursedSoulRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl != PlayerControl.LocalPlayer)
        {
            return;
        }
        ImportantTextTask orCreateTask = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
        orCreateTask.Text = $"{TownOfUsColors.Neutral.ToTextColor()}{TouLocale.GetParsed("NeutralBenignTaskHeader")}</color>";
        orCreateTask.name = "NeutralRoleText";
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<OracleRole>());
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string LocaleKey => "CursedSoul";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}SoulSwap", "Soul Swap"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}SoulSwapWikiDescription"),
                    ToUJKNeutAssets.CursedSoulSoulSwapSprite)
            };
        }
    }

    public Color RoleColor => TownOfUsMiraJKColors.CursedSoul;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.PhantomIntroSound,
        OptionsScreenshot = TouBanners.NeutralRoleBanner,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = ToUJKRoleIcons.CursedSoul
    };

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        TouRoleUtils.ClearTaskHeader(Player);
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return false;
    }

    [MethodRpc((uint)TownOfUsJKRpc.SoulSwap, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcSoulSwap(PlayerControl player, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        if (player.Data.Role is not CursedSoulRole)
        {
            Error("RpcRemember - Invalid cursed soul");
            return;
        }

        var opts = OptionGroupSingleton<CursedSoulOptions>.Instance;
        var targetRole = target.Data.Role;

        if (!IsRoleValid(targetRole))
        {
            return;
        }

        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.CursedSoulPreSoulSwap, player, target);
        MiraEventManager.InvokeEvent(touAbilityEvent);

        player.ChangeRole((ushort)targetRole.Role);
        if (player.Data.Role is InquisitorRole inquis)
        {
            inquis.Targets = ModifierUtils.GetPlayersWithModifier<InquisitorHereticModifier>().Where(x => x != player)
                .ToList();
            inquis.TargetRoles = ModifierUtils.GetActiveModifiers<InquisitorHereticModifier>()
                .Where(x => x.Player != player)
                .Select([HideFromIl2Cpp](x) => x.TargetRole).OrderBy([HideFromIl2Cpp](x) => x.GetRoleName()).ToList();
        }
        else if (player.Data.Role is PlaguebearerRole || player.Data.Role is PestilenceRole)
        {
            ModifierUtils.GetActiveModifiers<PlaguebearerInfectedModifier>()
                .Do(x => x.ModifierComponent?.RemoveModifier(x));
            player.AddModifier<PlaguebearerInfectedModifier>(player.PlayerId);
        }
        else if (player.Data.Role is BakerRole || player.Data.Role is FamineRole)
        {
            ModifierUtils.GetActiveModifiers<BakerFedModifier>()
                .Do(x => x.ModifierComponent?.RemoveModifier(x));
        }
        else if (player.Data.Role is ArsonistRole)
        {
            ModifierUtils.GetActiveModifiers<ArsonistDousedModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
        }
        else if (player.Data.Role is MayorRole mayor)
        {
            mayor.Revealed = false;
        }
        else if (player.Data.Role is FairyRole fairy)
        {
            var fairyMod = ModifierUtils.GetActiveModifiers<GuardianAngelTargetModifier>()
                .FirstOrDefault(x => x.OwnerId == target.PlayerId);

            if (fairyMod != null)
            {
                fairy.Target = fairyMod.Player;
                fairyMod.OwnerId = player.PlayerId;
            }
        }
        else if (player.Data.Role is ExecutionerRole exe)
        {
            var exeMod = ModifierUtils.GetActiveModifiers<ExecutionerTargetModifier>()
                .FirstOrDefault(x => x.OwnerId == target.PlayerId);

            if (exeMod != null)
            {
                exe.Target = exeMod.Player;
                exeMod.OwnerId = player.PlayerId;
            }
        }
        else if (player.Data.Role is VampireRole)
        {
            if (target.HasModifier<VampireBittenModifier>())
            {
                // Makes the amne stay with the bitten modifier
                player.AddModifier<VampireBittenModifier>();
            }
            else
            {
                // Makes the og vampire a bitten vampire so to speak, yes it makes it more confusing, but that's how it is, deal with it - Atony
                target.AddModifier<VampireBittenModifier>();
            }
        }

        var newRoleType = opts.SwappedPlayerBecomes switch
        {
            SwappedRole.Crew => (ushort)RoleTypes.Crewmate,
            SwappedRole.Jester => RoleId.Get<JesterRole>(),
            SwappedRole.Survivor => RoleId.Get<SurvivorRole>(),
            SwappedRole.Amnesiac => RoleId.Get<AmnesiacRole>(),
            SwappedRole.Mercenary => RoleId.Get<MercenaryRole>(),
            SwappedRole.CursedSoul => RoleId.Get<CursedSoulRole>(),
            SwappedRole.Anarchist => RoleId.Get<AnarchistRole>(),
            _ => (ushort)RoleTypes.Crewmate
        };

        target.ChangeRole(newRoleType);

        if (player.AmOwner)
        {
            var text = TouLocale.GetParsed("TouJKRoleCursedSoulSoulSwapOwnerNotif");
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{text.Replace("<role>", $"{player.Data.Role.TeamColor.ToTextColor()}{player.Data.Role.GetRoleName()}</color>")}</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.CursedSoul.LoadAsset());
            notif1.AdjustNotification();
        }

        if (target.AmOwner)
        {
            var text = TouLocale.GetParsed("TouJKRoleCursedSoulSoulSwapTargetNotif");
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{text.Replace("<role>", $"{TownOfUsMiraJKColors.CursedSoul.ToTextColor()}{target.Data.Role.GetRoleName()}</color>")}</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.CursedSoul.LoadAsset());
            notif1.AdjustNotification();
        }

        var playerIsAssassin = target.HasModifier<AssassinModifier>();

        var assassin = target.GetModifiers<AssassinModifier>().FirstOrDefault();
        if (opts.SwapAssassinModifier && assassin != null)
        {
            player.AddModifier(assassin.GetType());
            target.RemoveModifier(assassin);
        }

        var modifier = target.GetModifiers<TouGameModifier>().FirstOrDefault(x => x is not AssassinModifier &&
            (x is not DoubleShotModifier || playerIsAssassin));
        if (opts.SwapFactionModifier && modifier != null)
        {
            player.AddModifier(modifier.GetType());
            target.RemoveModifier(modifier);
        }

        var cover = target.GetModifiers<UndercoverCoverModifier>().FirstOrDefault();
        if (cover != null)
        {
            player.AddModifier<UndercoverCoverModifier>((ushort)cover.ShownRole!.Role);
            target.RemoveModifier(cover);
        }

        var touAbilityEvent2 = new TouAbilityEvent((AbilityType)JKAbilityType.CursedSoulPostSoulSwap, player, target);
        MiraEventManager.InvokeEvent(touAbilityEvent2);
    }
    public static bool IsRoleValid(RoleBehaviour role)
    {
        var opts = OptionGroupSingleton<CursedSoulOptions>.Instance;
        if (role is IGhostRole)
        {
            return false;
        }
        else if (role.IsImpostor())
        {
            return opts.SwapWithImpostor;
        }
        else if (role.GetRoleAlignment() == RoleAlignment.NeutralKilling && !role.IsApocalypse())
        {
            return opts.SwapWithNeutralKiller;
        }
        else if (role.IsApocalypse())
        {
            return opts.SwapWithNeutralApocalypse;
        }
        return true;
    }
}