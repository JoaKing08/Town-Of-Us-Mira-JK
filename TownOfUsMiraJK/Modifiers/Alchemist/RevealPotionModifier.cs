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
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Alchemist;

public sealed class RevealPotionModifier(PlayerControl alchemist) : RevealModifier(0, true, null), IPotionEffect
{
    public override string ModifierName => TouLocale.Get("TouJKRoleAlchemistPotionRevealPotionEffect");
    public override bool HideOnUi => false;
    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKCrewAssets.AlchemistPotionSprite;
    public bool CanAppear => true;
    public bool CanSelf => false;
    public string PotionName => TouLocale.Get("TouJKRoleAlchemistPotionRevealPotion");
    public PlayerControl Alchemist => alchemist;
    public override ChangeRoleResult ChangeRoleResult => ChangeRoleResult.UpdateInfo;
    public override string GetDescription()
    {
        return TouLocale.GetParsed("TouJKRoleAlchemistPotionDescription").Replace("<potion>", PotionName);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Visible = Alchemist.AmOwner;
    }
}