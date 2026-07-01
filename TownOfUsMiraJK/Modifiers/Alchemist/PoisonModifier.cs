using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Networking;
using TownOfUs.Options;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Alchemist;

public sealed class PoisonModifier(PlayerControl alchemist) : TimedModifier, IPotionEffect
{
    public override string ModifierName => TouLocale.Get("TouJKRoleAlchemistPotionPoisonEffect");
    public override bool HideOnUi => false;
    public override bool AutoStart => true;
    public override float Duration => OptionGroupSingleton<AlchemistOptions>.Instance.PotionDurationShort;
    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKCrewAssets.AlchemistPotionSprite;
    public bool CanAppear => true;
    public bool CanSelf => false;
    public string PotionName => TouLocale.Get("TouJKRoleAlchemistPotionPoison");
    public PlayerControl Alchemist => alchemist;
    public override string GetDescription()
    {
        return TouLocale.GetParsed("TouJKRoleAlchemistPotionDescriptionTimer").Replace("<time>", ((int)TimeRemaining).ToString()).Replace("<potion>", PotionName);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        if (!Player.HasDied())
        {
            Alchemist.RpcCustomMurder(Player, createDeadBody: !MeetingHud.Instance && !ExileController.Instance, teleportMurderer: false, showKillAnim: !MeetingHud.Instance && !ExileController.Instance, playKillSound: !MeetingHud.Instance && !ExileController.Instance);
        }
    }
}