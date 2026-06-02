using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Modifiers;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Game.Impostor;

public sealed class TaskerModifier : TouGameModifier, IWikiDiscoverable
{
    public override string LocaleKey => "Tasker";
    public override string ModifierName => TouLocale.Get($"TouJKModifier{LocaleKey}");
    public override string IntroInfo => "You can do tasks.";
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public override LoadableAsset<Sprite>? ModifierIcon => ModifierIcons.Tasker;
    public override ModifierFaction FactionType => ModifierFaction.ImpostorPassive;

    public float Timer { get; set; }

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
        return (int)OptionGroupSingleton<ImpostorModifierJKOptions>.Instance.TaskerChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<ImpostorModifierJKOptions>.Instance.TaskerAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor();
    }

    [HarmonyPatch]
    public static class CanUsePatch
    {
        [HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.CanUse))]
        [HarmonyPrefix]
        public static bool Postfix(IUsable usable, ImpostorRole __instance, ref bool __result)
        {
            if (!__instance.Player.HasModifier<TaskerModifier>())
            {
                return true;
            }
            if (usable.TryCast<ZiplineConsole>() != null || usable.TryCast<Ladder>() != null || usable.TryCast<PlatformConsole>() != null)
            {
                __result = true;
                return false;
            }
            if (usable.TryCast<Console>() == null)
            {
                __result = usable.TryCast<DoorConsole>() != null;
                return false;
            }
            __result = true;
            return false;
        }
    }
}