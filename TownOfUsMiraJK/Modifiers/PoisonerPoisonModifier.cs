using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modules.Localization;
using TownOfUs.Networking;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Impostor;
using UnityEngine;

namespace TownOfUs.Modifiers.Impostor;

public sealed class PoisonerPoisonModifier(byte poisonerId) : TimedModifier
{
    public override string ModifierName => "Poison";
    public override bool HideOnUi => true;
    public PlayerControl Poisoner { get; } = MiscUtils.PlayerById(poisonerId)!;
    public override float Duration => OptionGroupSingleton<PoisonerOptions>.Instance.PoisonDuration;
    public override bool AutoStart => true;
    public bool ShownPoison { get; set; }

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.PoisonerPoison, Poisoner, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        ShownPoison = OptionGroupSingleton<PoisonerOptions>.Instance.ShowPoison;
        if (!ShownPoison && OptionGroupSingleton<PoisonerOptions>.Instance.PoisonDelay.Value <= 0)
        {
            if (Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRolePoisonerPoisonTargetNotif").Replace("<role>", $"{TownOfUsColors.Impostor.ToTextColor()}{Poisoner.Data.Role.GetRoleName()}</color>").Replace("<time>", Duration.ToString("0")),
                    Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Poisoner.LoadAsset());

                notif1.AdjustNotification();
            }
            ShownPoison = true;
        }
    }

    public override void Update()
    {
        if (!ShownPoison && OptionGroupSingleton<PoisonerOptions>.Instance.PoisonDelay.Value <= Duration - TimeRemaining)
        {
            if (Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRolePoisonerPoisonTargetNotif").Replace("<role>", $"{TownOfUsColors.Impostor.ToTextColor()}{Poisoner.Data.Role.GetRoleName()}</color>").Replace("<time>", (Duration - OptionGroupSingleton<PoisonerOptions>.Instance.PoisonDelay.Value).ToString("0")),
                Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Poisoner.LoadAsset());

                notif1.AdjustNotification();
            }
            ShownPoison = true;
        }
    }

    public override void OnDeactivate()
    {
        if (Poisoner.AmOwner)
        {
            Poisoner.RpcSpecialMurder(Player, true, resetKillTimer: false, teleportMurderer: false, causeOfDeath: "Poisoner");

            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRolePoisonerPoisonDiedNotif").Replace("<player>", $"{TownOfUsColors.Impostor.ToTextColor()}{Player.Data.PlayerName}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Poisoner.LoadAsset());

            notif1.AdjustNotification();
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        ModifierComponent?.RemoveModifier(this);
    }
}