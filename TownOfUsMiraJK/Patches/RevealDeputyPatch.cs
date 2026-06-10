using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Options;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class DeputyRevealPatch
{
    [HarmonyPatch(typeof(DeputyRole), nameof(DeputyRole.ClickGuess))]
    [HarmonyPostfix]
    public static void Reveal(PlayerVoteArea voteArea, DeputyRole __instance)
    {
        if (!OptionGroupSingleton<TouMTweaksOptions>.Instance.DeputyReveal)
        {
            return;
        }

        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        if (__instance.Killer == target && !target.HasModifier<InvulnerabilityModifier>())
        {
            RpcDeputyReveal(__instance.Player);
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.DeputyReveal, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcDeputyReveal(PlayerControl deputy)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(deputy);
            return;
        }
        deputy.AddModifier<DeputyRevealModifier>(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<DeputyRole>()));
    }
}