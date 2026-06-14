using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Crewmate;

public sealed class PirateDuelModifier(PlayerControl pirate) : BaseModifier
{
    public override string ModifierName => "Duel";
    public override bool HideOnUi => true;
    public PlayerControl Pirate { get; } = pirate;
    public DuelOption ChosenOption { get; set; } = DuelOption.Scimitar_Sidestep;
    public DuelOption ChosenOptionPirate { get; set; } = DuelOption.Scimitar_Sidestep;
    public GameObject DuelButton { get; set; }
    public GameObject DuelButtonText { get; set; }

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.PirateDuel, Pirate, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }
    public override void OnDeactivate()
    {
        base.OnDeactivate();
        DuelButton?.DeepDestroy();
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = Player.AmOwner ? Player : Pirate;
        return voteArea?.TargetPlayerId != player.PlayerId || player.Data.IsDead || !ModifierUtils.GetActiveModifiers<PirateDuelModifier>(x => !x.Player.HasDied() && x.Pirate == Pirate).Any();
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
    public override void OnMeetingStart()
    {
        base.OnMeetingStart();

        ChosenOption = DuelOption.Scimitar_Sidestep;
        ChosenOptionPirate = DuelOption.Scimitar_Sidestep;
        if (!Player.HasDied() && ModifierUtils.GetActiveModifiers<PirateDuelModifier>(x => !x.Player.HasDied() && x.Pirate == Pirate).Any())
        {
            if (Pirate.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRolePirateDuelStartOwner").Replace("<player>", $"{TownOfUsMiraJKColors.Pirate.ToTextColor()}{ModifierUtils.GetPlayersWithModifier<PirateDuelModifier>(x => x.Pirate.PlayerId == x.Pirate.PlayerId).FirstOrDefault().Data?.PlayerName}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Pirate.LoadAsset());

                notif1.AdjustNotification();
                GenButton(MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == Pirate.PlayerId), DuelOption.Scimitar_Sidestep, true);
            }
            else if (Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRolePirateDuelStartTarget").Replace("<role>", $"{TownOfUsMiraJKColors.Pirate.ToTextColor()}{Pirate.Data.Role.GetRoleName()}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Pirate.LoadAsset());

                notif1.AdjustNotification();
                GenButton(MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == Player.PlayerId), DuelOption.Scimitar_Sidestep, false);
            }
        }
    }
    private void GenButton(PlayerVoteArea? voteArea, DuelOption currentOption = DuelOption.Scimitar_Sidestep, bool pirate = false)
    {
        if (!MeetingHud.Instance || voteArea == null)
        {
            return;
        }
        if (DuelButton == null)
        {
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;

            var newButtonObj = GameObject.Instantiate(confirmButton, voteArea.transform);
            newButtonObj.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButtonObj.transform.localScale *= 0.8f;
            newButtonObj.layer = 5;
            newButtonObj.transform.parent = confirmButton.transform.parent.parent;

            DuelButtonText = GameObject.Instantiate(
                MeetingHud.Instance.MeetingAbilityButton.buttonLabelText.gameObject,
                newButtonObj.transform);
            DuelButtonText.transform.localPosition = new Vector3(0, -0.2f, 0f);
            var tmpText = DuelButtonText.GetComponent<TextMeshPro>();
            tmpText.color = Color.white;
            tmpText.fontSize = 2.5f;
            tmpText.fontSizeMax = 2.5f;
            tmpText.fontSizeMin = 2.5f;
            tmpText.m_enableWordWrapping = false;

            DuelButton = newButtonObj;

            var passive = newButtonObj.GetComponent<PassiveButton>();
            passive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            passive.OnClick.AddListener(OnClick());
        }
        switch (currentOption)
        {
            case DuelOption.Scimitar_Sidestep:
                DuelButtonText.GetComponent<TextMeshPro>().text = pirate ? TouLocale.GetParsed("TouJKRolePirateDuelScimitar") : TouLocale.GetParsed("TouJKRolePirateDuelSidestep");
                DuelButton.GetComponent<SpriteRenderer>().sprite = pirate ? ToUJKNeutAssets.PirateScimitarSprite.LoadAsset() : ToUJKNeutAssets.PirateSidestepSprite.LoadAsset();
                break;
            case DuelOption.Rapier_Chainmail:
                DuelButtonText.GetComponent<TextMeshPro>().text = pirate ? TouLocale.GetParsed("TouJKRolePirateDuelRapier") : TouLocale.GetParsed("TouJKRolePirateDuelChainmail");
                DuelButton.GetComponent<SpriteRenderer>().sprite = pirate ? ToUJKNeutAssets.PirateRapierSprite.LoadAsset() : ToUJKNeutAssets.PirateChainmailSprite.LoadAsset();
                break;
            case DuelOption.Pistol_Backpedal:
                DuelButtonText.GetComponent<TextMeshPro>().text = pirate ? TouLocale.GetParsed("TouJKRolePirateDuelPistol") : TouLocale.GetParsed("TouJKRolePirateDuelBackpedal");
                DuelButton.GetComponent<SpriteRenderer>().sprite = pirate ? ToUJKNeutAssets.PiratePistolSprite.LoadAsset() : ToUJKNeutAssets.PirateBackpedalSprite.LoadAsset();
                break;
        }
    }
    [HideFromIl2Cpp]
    private Action OnClick()
    {
        void Listener()
        {
            var player = Player.AmOwner ? Player : Pirate;
            var option = Player.AmOwner ? ChosenOption : ChosenOptionPirate;
            switch (option)
            {
                case DuelOption.Scimitar_Sidestep:
                    option = DuelOption.Rapier_Chainmail;
                    break;
                case DuelOption.Rapier_Chainmail:
                    option = DuelOption.Pistol_Backpedal;
                    break;
                case DuelOption.Pistol_Backpedal:
                    option = DuelOption.Scimitar_Sidestep;
                    break;
            }
            PirateRole.RpcChangeDuelOption(Player, Pirate.AmOwner, (byte)option);
            GenButton(MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == player.PlayerId), option, Pirate.AmOwner);
        }

        return Listener;
    }
}
public enum DuelOption
{
    Scimitar_Sidestep,
    Rapier_Chainmail,
    Pistol_Backpedal
}