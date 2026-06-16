using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Voting;
using System.Reflection;
using TownOfUs.Events.Misc;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Options;
using TownOfUs.Patches.Options;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using UnityEngine;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch]
public static class SeeVotesPatch
{
    [HarmonyPatch(typeof(DeadSeeVoteColorsPatch), nameof(DeadSeeVoteColorsPatch.Prefix))]
    [HarmonyPrefix]
    public static bool Prefix([HarmonyArgument(0)] MeetingHud instance, [HarmonyArgument(1)] NetworkedPlayerInfo voterPlayer, [HarmonyArgument(2)] int index, [HarmonyArgument(3)] Transform parent)
    {
        var spriteRenderer = GameObject.Instantiate(instance.PlayerVotePrefab);
        var player = MiscUtils.PlayerById(voterPlayer.PlayerId);
        var suspect = parent.gameObject.GetComponent<PlayerVoteArea>();
        if (PlayerControl.LocalPlayer.Data.Role is ProsecutorRole)
        {
            if (player?.Data.Role is SecretaryRole secretary && secretary.VotedFor.Count > 0)
            {
                if (secretary.VotesShown > 1 + KnightedEvents.ExtraKnightVotes.Count(x => x.Voter == secretary.Player.PlayerId) || (secretary.VotedFor.Count > 0 && secretary.VotedFor.FirstOrDefault() != suspect.TargetPlayerId))
                {
                    PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
                }
                else
                {
                    PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
                    secretary.VotesShown++;
                }
            }
            else
            {
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
            }
        }
        else if (player != null && player.Data.Role is ProsecutorRole pros && pros.HasProsecuted &&
                 !PlayerControl.LocalPlayer.Data.IsDead)
        {
            PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
        }
        else if (GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes &&
                 (!OptionGroupSingleton<PostmortemOptions>.Instance.DeadSeeVotes || !PlayerControl.LocalPlayer.Data.IsDead))
        {
            PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
        }
        else
        {
            if (player?.Data.Role is SecretaryRole secretary && secretary.VotedFor.Count > 0)
            {
                if (secretary.VotesShown > 1 + KnightedEvents.ExtraKnightVotes.Count(x => x.Voter == secretary.Player.PlayerId) || (secretary.VotedFor.Count > 0 && secretary.VotedFor.FirstOrDefault() != suspect.TargetPlayerId))
                {
                    PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
                }
                else
                {
                    PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
                    secretary.VotesShown++;
                }
            }
            else
            {
                PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
            }
        }

        spriteRenderer.transform.SetParent(parent);
        spriteRenderer.transform.localScale = Vector3.zero;
        var component = parent.GetComponent<PlayerVoteArea>();
        if (component != null)
        {
            spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);
        }

        instance.StartCoroutine(Effects.Bloop(index * 0.3f, spriteRenderer.transform));
        parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
        return false;
    }
}