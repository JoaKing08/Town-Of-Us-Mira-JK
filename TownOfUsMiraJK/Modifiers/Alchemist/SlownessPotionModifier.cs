using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Modifiers.Neutral;

public sealed class SlownessPotionModifier(PlayerControl alchemist) : TimedModifier, IVisualAppearance, IPotionEffect
{
    public override string ModifierName => TouLocale.Get("TouJKRoleAlchemistPotionSlownessPotionEffect");
    public override bool HideOnUi => false;
    public override bool AutoStart => true;
    public override float Duration => OptionGroupSingleton<AlchemistOptions>.Instance.PotionDurationLong;
    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKCrewAssets.AlchemistPotionSprite;
    public bool CanAppear => true;
    public bool CanSelf => false;
    public string PotionName => TouLocale.Get("TouJKRoleAlchemistPotionSlownessPotion");
    public PlayerControl Alchemist => alchemist;
    public override string GetDescription()
    {
        return TouLocale.GetParsed("TouJKRoleAlchemistPotionDescriptionTimer").Replace("<time>", ((int)TimeRemaining).ToString()).Replace("<potion>", PotionName);
    }

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        appearance.Speed *= OptionGroupSingleton<AlchemistOptions>.Instance.SlownessPotion;
        return appearance;
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }
}