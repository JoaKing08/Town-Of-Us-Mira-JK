using InnerNet;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using System.Collections;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Roles.Other;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Impostor;

public sealed class DemagogueRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, IAssignableTargets
{
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string LocaleKey => "Demagogue";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");
    public PlayerControl? Immunity { get; set; }
    public bool ImmunityAlive => Immunity != null && !Immunity.HasDied();
    public bool Immunitized { get; set; }
    public bool EasterEgg { get; set; }

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => TownOfUsColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorPower;

    public CustomRoleConfiguration Configuration => new(this)
    {
        OptionsScreenshot = TouBanners.ImpostorRoleBanner,
        Icon = ToUJKRoleIcons.Demagogue
    };

    public int Priority { get; set; } = 2;

    public void AssignTargets()
    {
        if (!OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment)
        {
            return;
        }

        var dems = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => x.IsRole<DemagogueRole>() && !x.HasDied());

        foreach (var dem in dems)
        {
            var filtered = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => !x.HasDied() && x.IsNeutral() && !x.IsRole<JesterRole>() &&
                            !(x.IsRole<ExecutionerRole>() &&
                            OptionGroupSingleton<ExecutionerOptions>.Instance.OnTargetDeath == BecomeOptions.Jester) &&
                            !(x.IsRole<FairyRole>() &&
                            OptionGroupSingleton<FairyOptions>.Instance.OnTargetDeath == BecomeOptions.Jester) &&
                            !x.HasModifier<ExecutionerTargetModifier>() &&
                            !SpectatorRole.TrackedSpectators.Contains(x.Data.PlayerName)).ToList();

            if (filtered.Count == 0)
            {
                filtered = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => !x.IsRole<DemagogueRole>() && !x.HasDied() &&
                            x.IsImpostor() &&
                            !x.HasModifier<ExecutionerTargetModifier>() &&
                            !SpectatorRole.TrackedSpectators.Contains(x.Data.PlayerName)).ToList();
            }

            if (filtered.Count == 0)
            {
                filtered = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => !x.HasDied() && x.IsCrewmate() &&
                            !x.HasModifier<ExecutionerTargetModifier>() &&
                            !SpectatorRole.TrackedSpectators.Contains(x.Data.PlayerName)).ToList();
            }

            if (filtered.Count > 0)
            {
                System.Random rndIndex = new();
                var randomTarget = filtered[rndIndex.Next(0, filtered.Count)];

                RpcSetDemagogueImmunity(dem, randomTarget);
            }
        }
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (TutorialManager.InstanceExists && Immunity == null &&
            AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started && PlayerControl.LocalPlayer.IsHost())
        {
            Coroutines.Start(SetTutorialTargets(this));
        }
    }
    public override void OnMeetingStart()
    {
        if (!Player.HasModifier<DemagogueRevealModifier>())
        {
            Player.AddModifier<DemagogueRevealModifier>();
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.SetDemagogueImmunity)]
    public static void RpcSetDemagogueImmunity(PlayerControl player, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        if (player.Data.Role is not DemagogueRole role)
        {
            Error("RpcSetDemagogueImmunity - Invalid demagogue");
            return;
        }

        if (target == null)
        {
            return;
        }

        role.Immunity = target;

        target.AddModifier<DemagogueImmunityModifier>(player.PlayerId);
    }

    [MethodRpc((uint)TownOfUsJKRpc.DemagogueImmunitized)]
    public static void RpcDemagogueImmunitized(PlayerControl exiled, bool easterEgg)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(exiled);
            return;
        }
        var dem = exiled.Data.Role as DemagogueRole;

        if (dem != null)
        {
            dem.Immunitized = true;
            dem.EasterEgg = easterEgg;
        }
    }

    private static IEnumerator SetTutorialTargets(DemagogueRole dem)
    {
        yield return new WaitForSeconds(0.01f);
        dem.AssignTargets();
    }
}