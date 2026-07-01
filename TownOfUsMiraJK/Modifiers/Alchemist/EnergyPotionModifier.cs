using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Alchemist;

public sealed class EnergyPotionModifier(PlayerControl alchemist) : TimedModifier, IPotionEffect
{
    public override string ModifierName => TouLocale.Get("TouJKRoleAlchemistPotionEnergyPotionEffect");
    public override bool HideOnUi => false;
    public override bool AutoStart => true;
    public override float Duration => OptionGroupSingleton<AlchemistOptions>.Instance.PotionDurationLong;
    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKCrewAssets.AlchemistPotionSprite;
    public bool CanAppear => true;
    public bool CanSelf => OptionGroupSingleton<AlchemistOptions>.Instance.AllowSelf;
    public string PotionName => TouLocale.Get("TouJKRoleAlchemistPotionEnergyPotion");
    public PlayerControl Alchemist => alchemist;
    public override string GetDescription()
    {
        return TouLocale.GetParsed("TouJKRoleAlchemistPotionDescriptionTimer").Replace("<time>", ((int)TimeRemaining).ToString()).Replace("<potion>", PotionName);
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!Player.AmOwner)
        {
            return;
        }
        foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(Player.Data.Role) && !x.EffectActive && x.Timer > 0 && !x.TimerPaused))
        {
            button.Timer -= Time.fixedDeltaTime * (OptionGroupSingleton<AlchemistOptions>.Instance.EnergyPotion - 1f);
        }
    }
}