using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Voting;
using TownOfUs.Events.Misc;
using TownOfUs.Modifiers;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Events.Crewmate;

public static class SecretaryEvents
{
    [RegisterEvent(1)]
    public static void ProcessVotesEventHandler(ProcessVotesEvent @event)
    {
        var votes = @event.Votes.ToList();

        if (!GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes)
        {
            foreach (var secretary in CustomRoleUtils.GetActiveRolesOfType<SecretaryRole>())
            {
                if (secretary.StoredVote)
                {
                    var vote = votes.FirstOrDefault(x => x.Voter == secretary.Player.PlayerId && x.Suspect == MeetingHud.Instance.SkipVoteButton.TargetPlayerId);
                    votes.Remove(vote);
                }
            }
        }

        KnightedEvents.ExtraKnightVotes.Clear();
        if (KnightedEvents.ShowVotes)
        {
            return;
        }

        var baseExtraVotes = (int)OptionGroupSingleton<MonarchOptions>.Instance.VotesPerKnight;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            var knightModifiers = player.GetModifiers<KnightedModifier>()?.ToList();
            if (knightModifiers == null || knightModifiers.Count == 0)
                continue;

            var vote = votes.FirstOrDefault(v => v.Voter == player.PlayerId);
            if (vote == default)
                continue;

            var totalBonusVotes = knightModifiers.Count * baseExtraVotes;

            for (var i = 0; i < totalBonusVotes; i++)
            {
                var extraVote = new CustomVote(vote.Voter, vote.Suspect);
                votes.Add(extraVote);
                KnightedEvents.ExtraKnightVotes.Add(extraVote);
            }
        }

        @event.ExiledPlayer = VotingUtils.GetExiled(votes, out _);
    }

    [RegisterEvent(1000)]
    public static void BeforeLocalVoteEvent(BeforeVoteEvent @event)
    {
        // Players who are dead can no longer vote, and dead player can't be voted either
        var voteArea = @event.VoteArea;
        var votedPlayer = voteArea.GetPlayer();
        if (PlayerControl.LocalPlayer.HasDied() || votedPlayer != null && votedPlayer.HasDied())
        {
            @event.Cancel();
            return;
        }

        if (PlayerControl.LocalPlayer.Data.Role is not SecretaryRole secretary)
        {
            return;
        }

        if (voteArea.Parent.state is MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Results)
        {
            @event.Cancel();
            return;
        }

        if (voteArea != secretary.StoreButton && (secretary.VotesStored > 1 || !secretary.NormalVoteUsed))
        {
            SecretaryRole.RpcVote(PlayerControl.LocalPlayer, voteArea.TargetPlayerId);
            voteArea.Cancel();
            @event.Cancel();
            return;
        }

        if (voteArea == secretary.StoreButton)
        {
            SecretaryRole.RpcStore(PlayerControl.LocalPlayer);
        }
    }

    // This is so MiraAPI doesn't cancel the final Secretary vote if they already voted for that player
    [HarmonyPatch(typeof(PlayerVoteData),nameof(PlayerVoteData.VotedFor))]
    public static class PatchVotedFor
    {
        public static void Postfix(byte playerId, PlayerVoteData __instance, ref bool __result)
        {
            var voter = __instance.Owner;
            if (voter.Data.Role is not SecretaryRole secretary)
            {
                return;
            }
            if (__instance.Votes.Count(x => x.Suspect == playerId) > secretary.VotedFor.Count(x => x == playerId))
            {
                return;
            }
            __result = false;
            return;
        }
    }

    [RegisterEvent(-100)]
    public static void MeetingSelectEvent(MeetingSelectEvent @event)
    {
        if (PlayerControl.LocalPlayer.Data.Role is not SecretaryRole secretary)
        {
            return;
        }
        if (secretary.StoredVote || (secretary.VotesStored <= 0 && secretary.NormalVoteUsed))
        {
            return;
        }
        @event.AllowSelect = true;
    }

    [RegisterEvent(1)]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        if (!OptionGroupSingleton<MonarchOptions>.Instance.ShowKnightedVotes || !@event.VoteData.Owner.HasModifier<KnightedModifier>())
        {
            return;
        }
        if (@event.VoteData.Owner.Data.Role is not SecretaryRole secretary)
        {
            return;
        }
        if (secretary.VotedFor.Count == 0)
        {
            return;
        }
        for (int i = 0; i < (int)OptionGroupSingleton<MonarchOptions>.Instance.VotesPerKnight; i++)
        {
            if (@event.VoteData.Votes.Any(x => x.Suspect == @event.TargetId))
            {
                @event.VoteData.RemovePlayerVote(@event.TargetId);
            }
        }
    }

    [RegisterEvent]
    public static void VoteEvent(AfterVoteEvent @event)
    {
        if (PlayerControl.LocalPlayer.Data.Role is not SecretaryRole secretary)
        {
            return;
        }
        if (@event.VoteArea != secretary.StoreButton)
        {
            if (secretary.NormalVoteUsed)
            {
                secretary.VotesStored--;
            }
            return;
        }
        PlayerControl.LocalPlayer.GetVoteData().Votes.RemoveAll(x => x.Suspect == secretary.StoreButton!.TargetPlayerId);
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        if (!MeetingHud.Instance)
        {
            return;
        }
        var target = @event.Target;
        foreach (var secr in CustomRoleUtils.GetActiveRolesOfType<SecretaryRole>())
        {
            secr.VotesStored += secr.VotedFor.RemoveAll(x => x == target.PlayerId);
        }
    }

    [RegisterEvent(400)]
    public static void WrapUpEvent(EjectionEvent @event)
    {
        foreach (var secr in CustomRoleUtils.GetActiveRolesOfType<SecretaryRole>())
        {
            secr.Cleanup();
        }
    }
}