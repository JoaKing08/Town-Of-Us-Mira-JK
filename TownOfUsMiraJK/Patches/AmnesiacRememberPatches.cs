using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class AmnesiacRememberPatches
{
    [HarmonyPatch(typeof(AmnesiacRole), nameof(AmnesiacRole.RpcRemember))]
    [HarmonyPostfix]
    public static void Remember(PlayerControl player, PlayerControl target)
    {
        var cover = target.GetModifiers<UndercoverCoverModifier>().FirstOrDefault();
        if (cover != null)
        {
            player.AddModifier<UndercoverCoverModifier>((ushort)(cover.ShownRole?.Role ?? RoleTypes.Impostor));
        }
        if (player.HasModifier<DemagogueImmunityModifier>() && player.IsRole<DemagogueRole>())
        {
            player.RemoveModifier<DemagogueImmunityModifier>();
            target.AddModifier<DemagogueImmunityModifier>();
            player.GetRole<DemagogueRole>()!.Immunity = target;
        }
    }
}