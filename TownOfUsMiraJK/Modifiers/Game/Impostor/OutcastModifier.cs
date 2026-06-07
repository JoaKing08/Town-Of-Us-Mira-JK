using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Modifiers;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Game.Impostor;

public sealed class OutcastModifier : TouGameModifier, IWikiDiscoverable
{
    public override string LocaleKey => "Outcast";
    public override string ModifierName => TouLocale.Get($"TouJKModifier{LocaleKey}");
    public override string IntroInfo => TouLocale.Get($"TouJKModifier{LocaleKey}IntroBlurb");
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKModifierIcons.Outcast;
    public override ModifierFaction FactionType => ModifierFaction.ImpostorUtility;

    public override string GetDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescription");
    }

    public string GetAdvancedDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}WikiDescription") + MiscUtils.AppendOptionsText(GetType());
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<ImpostorModifierJKOptions>.Instance.OutcastChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<ImpostorModifierJKOptions>.Instance.OutcastChance > 0 ? 1 : 0;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor() && role.GetRoleAlignment() != TownOfUs.Roles.RoleAlignment.ImpostorPower;
    }
}