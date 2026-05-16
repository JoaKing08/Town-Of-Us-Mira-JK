using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.PluginLoading;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUs.Modifiers.Crewmate.Coroner;

[MiraIgnore]
public abstract class CoronerResultBaseModifier(DeadBody body) : BaseModifier
{
    public override bool Unique => false;
    public override string ModifierName => "Coroner Information";
    public override bool HideOnUi => true;
    public DeadBody? Body { get; } = body;
    public CoronerData? CoronerData { get; set; } = CoronerData.AllData.FirstOrDefault(x => x.VictimId == body.ParentId)?.Copy();

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.CoronerAutopsy, Player, Body);
        MiraEventManager.InvokeEvent(touAbilityEvent);

        if (Player.AmOwner)
        {
            var notifMessage = (OptionGroupSingleton<CoronerOptions>.Instance.InfoDuringRound ? NotificationMessage() : TouLocale.Get("TouJKRoleCoronerAutopsyNotif")).Replace("<player>", $"{Colors.Coroner.ToTextColor()}{CoronerData?.VictimName ?? ""}</color>");
            if (!string.IsNullOrWhiteSpace(notifMessage))
            {
                var notif1 = Helpers.CreateAndShowNotification(notifMessage, Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Coroner.LoadAsset());
                notif1.AdjustNotification();
            }
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        if (Player.AmOwner)
        {
            var meetingMessage = MeetingMessage().Replace("<player>", CoronerData?.VictimName ?? "");
            if (!string.IsNullOrWhiteSpace(meetingMessage))
            {
                MiscUtils.AddFakeChat(MiscUtils.PlayerById(Body.ParentId)?.Data ?? Player.Data, TouLocale.Get("TouJKRoleCoronerAutopsyTitle"), meetingMessage);
            }
        }
        ModifierComponent?.RemoveModifier(this);
    }

    public virtual string MeetingMessage()
    {
        return "";
    }

    public virtual string NotificationMessage()
    {
        return "";
    }
}