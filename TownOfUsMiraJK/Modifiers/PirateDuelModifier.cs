using MiraAPI.Events;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Networking;
using TownOfUs.Patches.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Crewmate;

public sealed class PirateDuelModifier(PlayerControl pirate) : BaseModifier
{
    public MeetingMenu meetingMenuScimitar;
    public MeetingMenu meetingMenuRapier;
    public MeetingMenu meetingMenuPistol;
    public override string ModifierName => "Duel";
    public override bool HideOnUi => true;
    public PlayerControl Pirate { get; } = pirate;
    public bool IsSelf => Player.PlayerId == Pirate.PlayerId;
    public DuelOption ChosenOption { get; set; } = DuelOption.Scimitar_Sidestep;

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.PirateDuel, Pirate, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);

        if (Player.AmOwner)
        {
            meetingMenuScimitar = new MeetingMenu(
                Player.Data.Role,
                ClickGuess,
                TouLocale.Get(IsSelf ? "TouJKRolePirateDuelScimitar" : "TouJKRolePirateDuelSidestep"),
                MeetingAbilityType.Click,
                IsSelf ? NeutAssets.PirateScimitarSprite : NeutAssets.PirateSidestepSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(1f, 0f, -3f)
            };
            meetingMenuRapier = new MeetingMenu(
                Player.Data.Role,
                ClickGuess,
                TouLocale.Get(IsSelf ? "TouJKRolePirateDuelRapier" : "TouJKRolePirateDuelChainmail"),
                MeetingAbilityType.Click,
                IsSelf ? NeutAssets.PirateRapierSprite : NeutAssets.PirateChainmailSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(1f, 0f, -3f)
            };
            meetingMenuPistol = new MeetingMenu(
                Player.Data.Role,
                ClickGuess,
                TouLocale.Get(IsSelf ? "TouJKRolePirateDuelPistol" : "TouJKRolePirateDuelBackpedal"),
                MeetingAbilityType.Click,
                IsSelf ? NeutAssets.PiratePistolSprite : NeutAssets.PirateBackpedalSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(1f, 0f, -3f)
            };
        }
    }
    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (Player.AmOwner)
        {
            meetingMenuScimitar?.Dispose();
            meetingMenuScimitar = null!;
            meetingMenuRapier?.Dispose();
            meetingMenuRapier = null!;
            meetingMenuPistol?.Dispose();
            meetingMenuPistol = null!;
        }
    }
    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        var option = DuelOption.Scimitar_Sidestep;
        meetingMenuScimitar.HideButtons();
        meetingMenuRapier.HideButtons();
        meetingMenuPistol.HideButtons();
        switch (ChosenOption)
        {
            case DuelOption.Scimitar_Sidestep:
                PirateRole.RpcChangeDuelOption(Player, (byte)DuelOption.Rapier_Chainmail);
                meetingMenuRapier.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && ModifierUtils.GetActiveModifiers<PirateDuelModifier>(x => !x.Player.HasDied() && x.Pirate == Pirate && x.Player.PlayerId != Player.PlayerId).Any());
                break;
            case DuelOption.Rapier_Chainmail:
                PirateRole.RpcChangeDuelOption(Player, (byte)DuelOption.Pistol_Backpedal);
                meetingMenuPistol.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && ModifierUtils.GetActiveModifiers<PirateDuelModifier>(x => !x.Player.HasDied() && x.Pirate == Pirate && x.Player.PlayerId != Player.PlayerId).Any());
                break;
            case DuelOption.Pistol_Backpedal:
                PirateRole.RpcChangeDuelOption(Player, (byte)DuelOption.Scimitar_Sidestep);
                meetingMenuScimitar.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && ModifierUtils.GetActiveModifiers<PirateDuelModifier>(x => !x.Player.HasDied() && x.Pirate == Pirate && x.Player.PlayerId != Player.PlayerId).Any());
                break;
            default:
                goto case DuelOption.Pistol_Backpedal;
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId != Player.PlayerId || Player.Data.IsDead || !ModifierUtils.GetActiveModifiers<PirateDuelModifier>(x => !x.Player.HasDied() && x.Pirate == Pirate && x.Player.PlayerId != Player.PlayerId).Any();
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
    public override void OnMeetingStart()
    {
        base.OnMeetingStart();

        if (Player.AmOwner)
        {
            ChosenOption = DuelOption.Scimitar_Sidestep;
            meetingMenuScimitar.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && ModifierUtils.GetActiveModifiers<PirateDuelModifier>(x => !x.Player.HasDied() && x.Pirate == Pirate && x.Player.PlayerId != Player.PlayerId).Any());
            if (!Player.HasDied() && ModifierUtils.GetActiveModifiers<PirateDuelModifier>(x => !x.Player.HasDied() && x.Pirate == Pirate && x.Player.PlayerId != Player.PlayerId).Any())
            {
                if (IsSelf)
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRolePirateDuelStartOwner").Replace("<player>", $"{Colors.Pirate.ToTextColor()}{ModifierUtils.GetPlayersWithModifier<PirateDuelModifier>(x => x.Pirate.PlayerId == x.Pirate.PlayerId && !x.IsSelf).FirstOrDefault().Data?.PlayerName}</color>"),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Pirate.LoadAsset());

                    notif1.AdjustNotification();
                }
                else
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRolePirateDuelStartTarget").Replace("<role>", $"{Colors.Pirate.ToTextColor()}{Pirate.Data.Role.GetRoleName()}</color>"),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Pirate.LoadAsset());

                    notif1.AdjustNotification();
                }
            }
        }
    }
}
public enum DuelOption
{
    Scimitar_Sidestep,
    Rapier_Chainmail,
    Pistol_Backpedal
}