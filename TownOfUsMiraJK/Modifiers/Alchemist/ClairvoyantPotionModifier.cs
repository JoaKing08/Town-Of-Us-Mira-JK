using MiraAPI.Events;
using MiraAPI.GameOptions;
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

public sealed class ClairvoyantPotionModifier(PlayerControl alchemist) : TimedModifier, IPotionEffect
{
    public override string ModifierName => TouLocale.Get("TouJKRoleAlchemistPotionClairvoyantPotionEffect");
    public override bool HideOnUi => false;
    public override bool AutoStart => true;
    public override float Duration => OptionGroupSingleton<AlchemistOptions>.Instance.PotionDurationShort;
    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKCrewAssets.AlchemistPotionSprite;
    public bool CanAppear => true;
    public bool CanSelf => OptionGroupSingleton<AlchemistOptions>.Instance.AllowSelf;
    public string PotionName => TouLocale.Get("TouJKRoleAlchemistPotionClairvoyantPotion");
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
        if (Player.AmOwner)
        {
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.IsDead && !x.Data.Disconnected))
            {
                player.Visible = true;
            }
        }
        base.FixedUpdate();
    }
}