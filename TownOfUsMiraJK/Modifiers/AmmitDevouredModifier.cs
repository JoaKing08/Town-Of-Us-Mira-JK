using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using Reactor.Utilities;
using TownOfUs.Events;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using UnityEngine;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class AmmitDevouredModifier(PlayerControl ammit) : DisabledModifier
{
    public override bool CanBeInteractedWith => false;
    public override bool CanUseAbilities => false;
    public override string ModifierName => "Devoured";

    public override bool HideOnUi => true;

    public PlayerControl Ammit { get; } = ammit;


    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.AmmitDevour, Ammit, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        if (Ammit?.AmOwner == true)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleAmmitDevourOwnerNotif").Replace("<player>", $"{TownOfUsMiraJKColors.Ammit.ToTextColor()}{Player?.Data.PlayerName}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Ammit.LoadAsset());

            notif1.AdjustNotification();
        }
        if (Player?.AmOwner == true)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleAmmitDevourTargetNotif").Replace("<player>", Ammit?.Data.PlayerName).Replace("<role>", $"{TownOfUsMiraJKColors.Ammit.ToTextColor()}{Ammit?.Data.Role.GetRoleName()}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Ammit.LoadAsset());

            notif1.AdjustNotification();
        }
        HidePlayer(Player!);
        Coroutines.Start(AmmitSizeModifier.CoUpdate(Ammit?.GetModifier<AmmitSizeModifier>()));
    }
    public override void OnDeactivate()
    {
        Player.transform.position = Ammit.transform.position;
        Coroutines.Start(AmmitSizeModifier.CoUpdate(Ammit?.GetModifier<AmmitSizeModifier>()));
        HidePlayer(Player, true);
    }

    public override void Update()
    {
        if (Player == null || Ammit == null)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }
        HidePlayer(Player);
        Player.transform.position = Ammit.transform.position;
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        if (!Player.HasModifier<InvulnerabilityModifier>())
        {
            DeathHandlerModifier.UpdateDeathHandlerImmediate(Player, TouLocale.Get("DiedToAmmit"),
                DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                lockInfo: DeathHandlerOverride.SetTrue);

            Player.Exiled();
        }
        ModifierComponent?.RemoveModifier(this);
    }
    public static void HidePlayer(PlayerControl player, bool show = false)
    {
        if (player == null)
        {
            return;
        }
        player.cosmetics.currentBodySprite.BodySprite.gameObject.SetActive(show);
        player.cosmetics.nameText.gameObject.SetActive(show);
        player.cosmetics.colorBlindText.gameObject.SetActive(show);
        player.cosmetics.hat.gameObject.SetActive(show);
        player.cosmetics.skin.gameObject.SetActive(show);
        player.cosmetics.visor.gameObject.SetActive(show);
        if (player.cosmetics.currentPet != null)
        {
            player.cosmetics.currentPet.gameObject.SetActive(show);
        }
        if (player.cosmetics.GetLongBoi() != null)
        {
            player.cosmetics.GetLongBoi().headSprite.gameObject.SetActive(show);
            player.cosmetics.GetLongBoi().neckSprite.gameObject.SetActive(show);
            player.cosmetics.GetLongBoi().foregroundNeckSprite.gameObject.SetActive(show);
        }
    }
    [HarmonyPatch]
    public static class GetClosestPlayersPatch
    {
        [HarmonyPatch(typeof(Helpers), nameof(Helpers.GetClosestPlayers), [typeof(Vector2), typeof(float), typeof(bool)])]
        [HarmonyPostfix]
        public static void Postfix(ref List<PlayerControl> __result)
        {
            __result = __result.Where(x => !x.HasModifier<AmmitDevouredModifier>()).ToList();
        }
    }
}