using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Alchemist;

public sealed class PerceptionPotionModifier(PlayerControl alchemist) : TimedModifier, IPotionEffect
{
    public override string ModifierName => TouLocale.Get("TouJKRoleAlchemistPotionPerceptionPotionEffect");
    public override bool HideOnUi => false;
    public override bool AutoStart => true;
    public override float Duration => OptionGroupSingleton<AlchemistOptions>.Instance.PotionDurationLong;
    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKCrewAssets.AlchemistPotionSprite;
    public bool CanAppear => true;
    public bool CanSelf => OptionGroupSingleton<AlchemistOptions>.Instance.AllowSelf;
    public string PotionName => TouLocale.Get("TouJKRoleAlchemistPotionPerceptionPotion");
    public PlayerControl Alchemist => alchemist;
    public override string GetDescription()
    {
        return TouLocale.GetParsed("TouJKRoleAlchemistPotionDescriptionTimer").Replace("<time>", ((int)TimeRemaining).ToString()).Replace("<potion>", PotionName);
    }

    public float VisionPerc { get; set; } = 1f;

    public override void OnActivate()
    {
        base.OnActivate();

        VisionPerc = 1f;
    }

    public override void OnDeath(DeathReason reason)
    {
        base.OnDeath(reason);

        ModifierComponent!.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        var opts = OptionGroupSingleton<AlchemistOptions>.Instance;

        if (TimeRemaining > opts.PotionDurationLong - 1f)
        {
            VisionPerc = Mathf.Lerp(1f, opts.PerceptionPotion, opts.PotionDurationLong - TimeRemaining);
        }
        else if (TimeRemaining < 1f)
        {
            VisionPerc = Mathf.Lerp(1f, opts.PerceptionPotion, TimeRemaining);
        }
        else
        {
            VisionPerc = opts.PerceptionPotion;
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        VisionPerc = 1f;
    }
}