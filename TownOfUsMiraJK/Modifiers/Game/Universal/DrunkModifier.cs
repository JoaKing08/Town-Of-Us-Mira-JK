using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.ModifierDisplay;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options.Modifiers;
using TownOfUs.Options.Modifiers.Universal;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles.Impostor;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Modifiers;
using TownOfUsMiraJK.Options.Modifiers.Impostor;
using TownOfUsMiraJK.Options.Modifiers.Universal;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Game.Universal;

public sealed class DrunkModifier : UniversalGameModifier, IWikiDiscoverable, IColoredModifier
{
    public override string LocaleKey => "Drunk";
    public override string ModifierName => TouLocale.Get($"TouJKModifier{LocaleKey}");
    public override string IntroInfo => "Your movement is inverted.";
    public override LoadableAsset<Sprite>? ModifierIcon => ModifierIcons.Drunk;

    public override ModifierFaction FactionType => ModifierFaction.UniversalVisibility;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);
    public int RoundsLeft { get; set; } = OptionGroupSingleton<DrunkOptions>.Instance.DrunkRounds;
    public override bool HideOnUi => !DrunkActive();
    public Color ModifierColor => Colors.Drunk;

    public override string GetDescription()
    {
        return OptionGroupSingleton<DrunkOptions>.Instance.DrunkExpires ? TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescriptionExpires").Replace("<time>", RoundsLeft.ToString()) : TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescription");
    }

    public string GetAdvancedDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}WikiDescription") + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<UniversalModifierJKOptions>.Instance.DrunkChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<UniversalModifierJKOptions>.Instance.DrunkAmount;
    }

    public bool DrunkActive()
    {
        return !OptionGroupSingleton<DrunkOptions>.Instance.DrunkExpires || RoundsLeft > 0;
    }

    [HarmonyPatch]
    public static class PlayerPhysicsPatch
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        [HarmonyPostfix]
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.TryGetModifier<DrunkModifier>(out var drunk) && __instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove && drunk.DrunkActive())
            {
                __instance.body.velocity *= -1f;
            }
        }
    }
    public override void OnMeetingStart()
    {
        RoundsLeft--;
        if (Player.AmOwner)
        {
            ModifierDisplayComponent.Instance?.RefreshModifiers();
        }
    }
}