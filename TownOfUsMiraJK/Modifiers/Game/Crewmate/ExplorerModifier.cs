using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Modifiers;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Game.Crewmate;

public sealed class ExplorerModifier : TouGameModifier, IWikiDiscoverable, IColoredModifier
{
    public override string LocaleKey => "Explorer";
    public override string ModifierName => TouLocale.Get($"TouJKModifier{LocaleKey}");
    public override string IntroInfo => TouLocale.Get($"TouJKModifier{LocaleKey}IntroBlurb");
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);

    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKModifierIcons.Explorer;
    public override ModifierFaction FactionType => ModifierFaction.CrewmateUtility;
    public Color ModifierColor => TownOfUsMiraJKColors.Explorer;


    public override string GetDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescription");
    }

    public string GetAdvancedDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}WikiDescription") + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CrewmateModifierJKOptions>.Instance.ExplorerChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<CrewmateModifierJKOptions>.Instance.ExplorerAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate() && !role.CanVent && role is not EngineerTouRole;
    }

    public override bool? CanVent()
    {
        return true;
    }
}