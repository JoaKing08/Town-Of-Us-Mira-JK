using HarmonyLib;
using MiraAPI.GameOptions;
using System.Collections.ObjectModel;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modules.Components;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class GetModdedOptionsForRolePatch
{
    [HarmonyPatch(typeof(MiscUtils), nameof(MiscUtils.GetModdedOptionsForRole))]
    [HarmonyPrefix]
    public static bool StartPatch(Type classType, ref ReadOnlyCollection<IModdedOption>? __result)
    {
        var optionGroups =
            AccessTools.Field(typeof(ModdedOptionsManager), "Groups").GetValue(null) as List<AbstractOptionGroup>;

        var options = optionGroups?.Where(x => x.OptionableType == classType)?.SelectMany(x => x.Children);
        __result = options is IList<IModdedOption> ? null : new ReadOnlyCollection<IModdedOption>(options as IList<IModdedOption>);
        return false;
    }
}