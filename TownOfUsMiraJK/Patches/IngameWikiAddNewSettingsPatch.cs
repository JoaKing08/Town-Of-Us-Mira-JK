using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class IngameWikiAddNewSettingsPatch
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
}