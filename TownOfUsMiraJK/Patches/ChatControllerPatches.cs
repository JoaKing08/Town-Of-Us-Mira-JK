using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Collections;
using TMPro;
using TownOfUs.Assets;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Patches.Misc;
using TownOfUs.Patches.Options;
using TownOfUs.Patches.Roles;
using TownOfUsMiraJK.Modifiers.Alchemist;
using UnityEngine;

namespace TownOfUsMiraJK.Patches;

// used for jailer and blackmailed players.
[HarmonyPatch(typeof(ChatControllerPatches), nameof(ChatControllerPatches.UpdateChatModePostfix))]
public static class ChatControllerPatchesPatches
{
    private static TextMeshPro? _noticeText
    {
        get
        {
            return (TextMeshPro?)typeof(ChatControllerPatches).GetField("_noticeText")?.GetValue(null);
        }
        set
        {
            typeof(ChatControllerPatches).GetField("_noticeText")?.SetValue(null, value);
        }
    }
    public static bool Prefix([HarmonyArgument(0)] ChatController instance)
    {
        if (_noticeText == null || !PlayerControl.LocalPlayer)
        {
            return false;
        }

        if (!MeetingHud.Instance)
        {
            if (_noticeText.text != string.Empty)
            {
                _noticeText.text = string.Empty;
            }

            return false;
        }

        if (PlayerControl.LocalPlayer.HasModifier<BlackmailedModifier>() &&
            !PlayerControl.LocalPlayer.Data.IsDead)
        {
            _noticeText.text = "You have been blackmailed.";
            instance.freeChatField.SetVisible(false);
            instance.quickChatField.SetVisible(false);
        }
        else if (PlayerControl.LocalPlayer.HasModifier<SilencePotionModifier>() &&
            !PlayerControl.LocalPlayer.Data.IsDead)
        {
            _noticeText.text = "You have been silenced.";
            instance.freeChatField.SetVisible(false);
            instance.quickChatField.SetVisible(false);
        }
        else if (TeamChatPatches.TeamChatActive && !PlayerControl.LocalPlayer.Data.IsDead)
        {
            _noticeText.text = string.Empty;
            instance.freeChatField.SetVisible(true);
            instance.quickChatField.SetVisible(false);
        }
        else if (PlayerControl.LocalPlayer.HasModifier<JailedModifier>() &&
                 !PlayerControl.LocalPlayer.Data.IsDead && !TeamChatPatches.TeamChatActive)
        {
            var canChat = OptionGroupSingleton<JailorOptions>.Instance.JaileePublicChat;
            if (canChat)
            {
                _noticeText.text = "You are jailed. You can use public chat.";
                instance.freeChatField.SetVisible(true);
            }
            else
            {
                _noticeText.text = "You are jailed. You cannot use public chat.";
                instance.freeChatField.SetVisible(false);
                instance.quickChatField.SetVisible(false);
            }
        }
        else
        {
            instance.freeChatField.SetVisible(true);
            _noticeText.text = string.Empty;
        }
        return false;
    }
}