using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class IngameWikiAddNewEntriesPatch
{
    [HarmonyPatch(typeof(IngameWikiMinigame), nameof(IngameWikiMinigame.AddNewSettings))]
    [HarmonyPostfix]
    public static void AddOwnSettings(IngameWikiMinigame instance)
    {
        instance._activeSettings.Add(new OptionWikiInfo("TouJKWikiSettingsGeneralSettingsTitle",
            new List<AbstractOptionGroup>()
            {
                OptionGroupSingleton<GeneralOptions>.Instance, OptionGroupSingleton<TouMTweaksOptions>.Instance
            }, ToUJKRoleIcons.Demagogue));
    }
    [HarmonyPatch(typeof(IngameWikiMinigame), nameof(IngameWikiMinigame.AddNewTerms))]
    [HarmonyPostfix]
    public static void AddOwnTerms(IngameWikiMinigame instance)
    {
        instance._activeTerms.Insert(3, new TermWikiInfo("TouJKTermsPotionEffectSymbolsTitle", "TouJKTermsPotionEffectSymbolsInfo", ToUJKRoleIcons.Alchemist));
    }
}