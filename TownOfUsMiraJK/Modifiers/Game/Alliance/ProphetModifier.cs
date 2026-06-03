using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using TownOfUs.Events;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Roles.Other;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Modifiers;
using TownOfUsMiraJK.Options.Modifiers.Alliance;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Game.Alliance;

public sealed class ProphetModifier : AllianceGameModifier, IWikiDiscoverable, IAssignableTargets, IColoredModifier
{
    public override string LocaleKey => "Prophet";
    public override string ModifierName => TouLocale.Get($"TouJKModifier{LocaleKey}");
    public string ShortName => TouLocale.Get($"TouJKModifier{LocaleKey}ShortName");
    public override string IntroInfo => TouLocale.GetParsed($"TouJKModifier{LocaleKey}IntroBlurb");

    public override string GetDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescription");
    }

    public string GetAdvancedDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}WikiDescription") + MiscUtils.AppendOptionsText(GetType());
    }

    public override string Symbol => "*";
    public override float IntroSize => 4f;
    public override bool DoesTasks => false;
    public override bool GetsPunished => false;
    public override bool CrewContinuesGame => false;
    public override ModifierFaction FactionType => ModifierFaction.CrewmateAlliance;
    public override AlliedFaction TrueFactionType => (AlliedFaction)9;

    public override bool CountTowardsTrueFaction =>
        OptionGroupSingleton<ProphetOptions>.Instance.ProphetReplacesApocalypse.Value;
    public override Color FreeplayFileColor => new Color32(220, 220, 220, 255);
    public override LoadableAsset<Sprite>? ModifierIcon => ModifierIcons.Prophet;

    public int Priority { get; set; } = -1;
    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public void AssignTargets()
    {
        if (!OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment)
        {
            return;
        }

        System.Random rnd = new();
        var chance = rnd.Next(1, 101);

        if (chance <=
            (int)OptionGroupSingleton<AllianceModifierJKOptions>.Instance.ProphetChance)
        {
            var filtered = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => x.IsCrewmate() &&
                            !x.HasDied() &&
                            !SpectatorRole.TrackedSpectators.Contains(x.Data.PlayerName) &&
                            (x.Data.Role is not ILoyalCrewmate loyalCrew || loyalCrew.CanBeCrewpostor) &&
                            !x.HasModifier<AllianceGameModifier>() &&
                            !x.HasModifier<ExecutionerTargetModifier>()).ToList();

            if (filtered.Count == 0)
            {
                return;
            }

            var randomTarget = filtered[rnd.Next(0, filtered.Count)];

            var apocs = Helpers.GetAlivePlayers().Where(x => x.IsApocalypse()).ToList();
            if (OptionGroupSingleton<ProphetOptions>.Instance.ProphetReplacesApocalypse.Value && apocs.Count > 1)
            {
                var textlognotfound = $"Replacing an apocalypse with a crewmate. Apocalypse members: {apocs.Count}.";
                MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Error, textlognotfound);
                var discardedApoc = apocs.Where(x => x.Data.Role is not ISpawnChange).Random();
                bool asNeutral;
                var randomInt = UnityEngine.Random.RandomRangeInt(0, 10);
                if (randomInt < 4)
                {
                    asNeutral = true;
                }
                else
                {
                    asNeutral = false;
                }

                var roles = MiscUtils.GetPotentialRoles().Where(x =>
                    ((x.IsCrewmate() && !asNeutral) || (x.IsNeutral() && !asNeutral)) &&
                    !Helpers.GetAlivePlayers().Any(y => y.Data.Role.Role == x.Role)).ToList();

                var checktext = $"Forcing {discardedApoc.Data.PlayerName} into a crewmate/neutral role.";
                MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Error, checktext);
                var newRole = RoleTypes.Crewmate;
                if (roles.Count == 0)
                {
                    var newtext = $"Forcing {discardedApoc.Data.PlayerName} into Crewmate.";
                    MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Error, newtext);
                }
                else
                {
                    var chosenRole = roles.Random()!;
                    newRole = chosenRole.Role;

                    var newtext = $"Forcing {discardedApoc.Data.PlayerName} into {chosenRole.GetRoleName()}.";
                    MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Error, newtext);
                }
                RpcSetUpProphet(randomTarget, discardedApoc, newRole);
            }
            else
            {
                var textlognotfound =
                    $"Could not replace an apocalypse with a crewmate. | Can Replace: {OptionGroupSingleton<ProphetOptions>.Instance.ProphetReplacesApocalypse.Value}, Enough Apocalypse: {apocs.Count > 1}";
                MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Error, textlognotfound);
                RpcSetUpProphet(randomTarget, randomTarget, RoleTypes.Crewmate);
            }
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.SetUpProphet)]
    public static void RpcSetUpProphet(PlayerControl newProphet, PlayerControl discardedApoc, RoleTypes newRole)
    {
        newProphet.AddModifier<ProphetModifier>();
        if (newProphet.PlayerId == discardedApoc.PlayerId)
        {
            return;
        }
        foreach (var mod in discardedApoc.GetModifiers<TouGameModifier>())
        {
            var faction = mod.FactionType.ToString();
            if (faction.Contains("Impostor") && !faction.Contains("Non"))
            {
                discardedApoc.RemoveModifier(mod);
            }
        }

        if (AmongUsClient.Instance.AmHost)
        {
            discardedApoc.RpcSetRole(newRole, true);
        }
    }

    public override int GetAmountPerGame()
    {
        return 0;
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        if (Player.AmOwner)
        {
            // player is meant to be crewmate, and crewmates don't have a task header!
            TouRoleUtils.ClearTaskHeader(PlayerControl.LocalPlayer);
        }
        if (!Player.HasModifier<BasicGhostModifier>())
        {
            Player.AddModifier<BasicGhostModifier>();
        }

        if (Player.HasModifier<ToBecomeTraitorModifier>())
        {
            Player.RemoveModifier<ToBecomeTraitorModifier>();
        }
    }

    public override int CustomAmount => (int)OptionGroupSingleton<AllianceModifierJKOptions>.Instance.ProphetChance != 0 ? 1 : 0;
    public override int CustomChance => (int)OptionGroupSingleton<AllianceModifierJKOptions>.Instance.ProphetChance;

    public static bool ProphetVisibilityFlag(PlayerControl player)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var isApoc = PlayerControl.LocalPlayer.IsApocalypse();

        return !player.HasModifier<TraitorCacheModifier>() && (player.AmOwner || player.Data != null && !player.Data.Disconnected && isApoc);
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate() &&
               (role is not ILoyalCrewmate loyalCrew || loyalCrew.CanBeCrewpostor);
    }

    public override bool? DidWin(GameOverReason reason)
    {
        return ApocalypseUtils.ApocalypseWinConditionMet();
    }

    public Color ModifierColor => Colors.Apocalypse;
}