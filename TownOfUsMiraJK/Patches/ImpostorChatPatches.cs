using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using System.Collections;
using TownOfUs.Assets;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Patches.Options;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Modifiers.Game.Impostor;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;
using static TownOfUs.Patches.Options.TeamChatPatches;

namespace TownOfUs.Patches.Roles;

[HarmonyPatch(typeof(TeamChatManager), nameof(TeamChatManager.RegisterBuiltInChats))]
public static class RegisterBuiltInChats
{
    private static bool _touJKChatsRegistered;
    public static void Postfix()
    {
        if (_touJKChatsRegistered)
        {
            return;
        }

        var impChat = ExtensionTeamChatRegistry.RegisteredHandlers.First(x => x.Priority == 30);
        var impChatIdx = ExtensionTeamChatRegistry.RegisteredHandlers.IndexOf(impChat);
        impChat.IsChatAvailable = () =>
            MeetingHud.Instance != null &&
            (PlayerControl.LocalPlayer.IsImpostorAligned() ||
            PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<UndercoverRole>())) &&
            OptionGroupSingleton<GeneralOptions>.Instance is { FFAImpostorMode: false, ImpostorChat.Value: true } &&
            !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>();
        // TO BE ABSOLUTELY SURE THE VALUE IS CHANGED
        ExtensionTeamChatRegistry.RegisteredHandlers[impChatIdx] = impChat;

        var apocalypseHandler = new ExtensionTeamChatHandler
        {
            Priority = 50,
            IsForced = false,
            IsChatAvailable = delegate
            {
                var instance = OptionGroupSingleton<GeneralJKOptions>.Instance;
                return (bool)MeetingHud.Instance && PlayerControl.LocalPlayer.IsApocalypseAligned() && instance is { ApocChat.Value: true, ApocTeam.Value: true };
            },
            SendMessage = RpcSendApocTeamChat,
            GetDisplayText = () => "Apocalypse Chat",
            DisplayTextColor = TownOfUsMiraJKColors.Apocalypse
        };
        ExtensionTeamChatRegistry.RegisterHandler(apocalypseHandler);

        var undeadHandler = new ExtensionTeamChatHandler
        {
            Priority = 60,
            IsForced = false,
            IsChatAvailable = delegate
            {
                var instance = OptionGroupSingleton<NecromancerOptions>.Instance;
                return (bool)MeetingHud.Instance && (PlayerControl.LocalPlayer.HasModifier<NecromancerUndeadModifier>() || PlayerControl.LocalPlayer.IsRole<NecromancerRole>()) && instance.UndeadChat;
            },
            SendMessage = RpcSendUndeadTeamChat,
            GetDisplayText = () => "Undead Chat",
            DisplayTextColor = TownOfUsMiraJKColors.Necromancer
        };
        ExtensionTeamChatRegistry.RegisterHandler(undeadHandler);
        _touJKChatsRegistered = true;
    }
    [MethodRpc((uint)TownOfUsJKRpc.SendApocTeamChat)]
    public static void RpcSendApocTeamChat(PlayerControl player, string text)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        var shouldMarkUnread = false;
        if ((PlayerControl.LocalPlayer.IsApocalypseAligned()) ||
            (DeathHandlerModifier.IsFullyDead(PlayerControl.LocalPlayer) && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow))
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{TownOfUsMiraJKColors.Apocalypse.ToHtmlStringRGBA()}>{TouLocale.GetParsed("TouJKApocalypseChatTitle").Replace("<player>", player.Data.PlayerName)}</color>",
                text, bubbleType: (BubbleType)6, onLeft: !player.AmOwner);
            shouldMarkUnread = true;
        }

        if (shouldMarkUnread && MeetingHud.Instance)
        {
            var chats = TeamChatManager.GetAllAvailableChats();
            var hasForcedChat = chats.Any(c => c.IsForced);
            var currentChat = CurrentChatIndex >= 0 && CurrentChatIndex < chats.Count ? chats[CurrentChatIndex] : null;
            if ((!TeamChatActive || currentChat == null || currentChat.Priority != 50) && !hasForcedChat)
            {
                TeamChatManager.MarkChatAsUnread(50);
            }
        }
    }
    [MethodRpc((uint)TownOfUsJKRpc.SendUndeadTeamChat)]
    public static void RpcSendUndeadTeamChat(PlayerControl player, string text)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        var shouldMarkUnread = false;
        if (PlayerControl.LocalPlayer.HasModifier<NecromancerUndeadModifier>() || PlayerControl.LocalPlayer.IsRole<NecromancerRole>() ||
            (DeathHandlerModifier.IsFullyDead(PlayerControl.LocalPlayer) && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow))
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{TownOfUsMiraJKColors.Necromancer.ToHtmlStringRGBA()}>{TouLocale.GetParsed("TouJKUndeadChatTitle").Replace("<player>", player.Data.PlayerName)}</color>",
                text, bubbleType: (BubbleType)7, onLeft: !player.AmOwner);
            shouldMarkUnread = true;
        }

        if (shouldMarkUnread && MeetingHud.Instance)
        {
            var chats = TeamChatManager.GetAllAvailableChats();
            var hasForcedChat = chats.Any(c => c.IsForced);
            var currentChat = CurrentChatIndex >= 0 && CurrentChatIndex < chats.Count ? chats[CurrentChatIndex] : null;
            if ((!TeamChatActive || currentChat == null || currentChat.Priority != 60) && !hasForcedChat)
            {
                TeamChatManager.MarkChatAsUnread(60);
            }
        }
    }
}

[HarmonyPatch(typeof(MiscUtils), "BouncePrivateChatDot")]
class BouncePrivateChatDotPatch
{
    public static void Postfix(BubbleType bubbleType, ref IEnumerator __result)
    {
        if (bubbleType != (BubbleType)6)
        {
            return;
        }
        IEnumerator GetEnumerator()
        {
            if (PrivateChatDot == null)
            {
                CreateTeamChatBubble();
            }
            var sprite = PrivateChatDot!.GetComponent<SpriteRenderer>();
            sprite.enabled = true;
            sprite.sprite = ToUJKAssets.ApocBubble?.LoadAsset() ?? TouChatAssets.ImpBubble.LoadAsset();
            yield return Effects.Bounce(sprite.transform, 0.3f, 0.125f);
        }
        __result = GetEnumerator();
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

[HarmonyPatch(typeof(SetNamePatch), nameof(SetNamePatch.SetNamePostfix))]
public static class SetNamePatchPatch
{
    public static bool Prefix([HarmonyArgument(0)] ChatBubble __instance, [HarmonyArgument(1)] string playerName, [HarmonyArgument(2)] Color color)
    {
        var player = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(x => x.Data.PlayerName == playerName);
        if (player == null) return false;
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        if ((genOpt.FFAImpostorMode || PlayerControl.LocalPlayer.HasModifier<OutcastModifier>() || player.HasModifier<OutcastModifier>()) && PlayerControl.LocalPlayer.IsImpostorAligned() && !DeathHandlerModifier.IsFullyDead(PlayerControl.LocalPlayer) &&
            !player.AmOwner && player.IsImpostorAligned() && MeetingHud.Instance)
        {
            __instance.NameText.color = Color.white;
        }
        else if (!genOpt.FFAImpostorMode && !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>() && PlayerControl.LocalPlayer.IsImpostorAligned() && !DeathHandlerModifier.IsFullyDead(PlayerControl.LocalPlayer) && player.IsRole<UndercoverRole>())
        {
            __instance.NameText.color = player.GetModifier<UndercoverCoverModifier>()?.ShownRole?.NameColor ?? Palette.ImpostorRed;
        }
        else if (color == Color.white &&
                 (player.AmOwner || player.Data.Role is MayorRole mayor && mayor.Revealed ||
                  DeathHandlerModifier.IsFullyDead(PlayerControl.LocalPlayer) && genOpt.TheDeadKnow) && PlayerControl.AllPlayerControls
                     .ToArray()
                     .FirstOrDefault(x => x.Data.PlayerName == playerName) && MeetingHud.Instance)
        {
            __instance.NameText.color = (player.GetRoleWhenAlive() is ICustomRole custom) ? custom.RoleColor : player.GetRoleWhenAlive().TeamColor;
        }
        return false;
    }
}