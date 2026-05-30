using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities.Extensions;
using TownOfUs.Buttons.Impostor;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Impostor.Herbalist;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Options.Maps;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Patches.Options;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Impostor;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Modifiers.Game.Impostor;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;
using static TownOfUs.Patches.Options.TeamChatPatches;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUs.Patches.Roles;

[HarmonyPatch(typeof(TeamChatManager), nameof(TeamChatManager.RegisterBuiltInChats))]
public static class RegisterBuiltInChats
{
    public static void Postfix()
    {
        var impChat = ExtensionTeamChatRegistry.RegisteredHandlers.First(x => x.Priority == 30);
        var impChatIdx = ExtensionTeamChatRegistry.RegisteredHandlers.IndexOf(impChat);
        impChat.IsChatAvailable = () =>
        {
            var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
            return MeetingHud.Instance != null &&
                   (PlayerControl.LocalPlayer.IsImpostorAligned() || PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<UndercoverRole>())) &&
                   genOpt is { FFAImpostorMode: false, ImpostorChat.Value: true } && !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>();
        };
        // TO BE ABSOLUTELY SURE THE VALUE IS CHANGED
        ExtensionTeamChatRegistry.RegisteredHandlers[impChatIdx] = impChat;
    }
}

[HarmonyPatch(typeof(TeamChatPatches), nameof(TeamChatPatches.RpcSendImpTeamChat))]
public static class RpcSendImpTeamChat
{
    public static bool Prefix(PlayerControl player, string text)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return false;
        }
        var shouldMarkUnread = false;
        if ((PlayerControl.LocalPlayer.IsImpostorAligned() && !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>()) ||
            (DeathHandlerModifier.IsFullyDead(PlayerControl.LocalPlayer) && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow))
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{TownOfUsColors.ImpSoft.ToHtmlStringRGBA()}>{TouLocale.GetParsed("ImpostorChatTitle").Replace("<player>", player.Data.PlayerName)}</color>",
                text, bubbleType: BubbleType.Impostor, onLeft: !player.AmOwner);
            shouldMarkUnread = true;
        }
        else if (PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<UndercoverRole>()))
        {
            MiscUtils.AddTeamChat(PlayerControl.LocalPlayer.Data,
                $"<color=#{TownOfUsColors.ImpSoft.ToHtmlStringRGBA()}>{TouLocale.GetParsed("ImpostorChatTitle").Replace("<player>", player.AmOwner ? player.Data.PlayerName : "Anonymous")}</color>",
                text, bubbleType: BubbleType.Impostor, onLeft: !player.AmOwner);
            shouldMarkUnread = true;
        }

        if (shouldMarkUnread && MeetingHud.Instance != null)
        {
            var chats = TeamChatManager.GetAllAvailableChats();
            var hasForcedChat = chats.Any(c => c.IsForced);
            var currentChat = CurrentChatIndex >= 0 && CurrentChatIndex < chats.Count ? chats[CurrentChatIndex] : null;
            if ((!TeamChatActive || currentChat == null || currentChat.Priority != 30) && !hasForcedChat)
            {
                TeamChatManager.MarkChatAsUnread(30);
            }
        }
        return false;
    }
}