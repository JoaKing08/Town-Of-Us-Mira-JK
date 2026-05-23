using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs;
using TownOfUs.Events;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Impostor;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Patches;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHudTimerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.UpdateTimerText))]
    [HarmonyBefore(TownOfUsPlugin.Id)]
    public static void TimerUpdatePostfix(MeetingHud __instance)
    {
        var newText = string.Empty;
        if (!PlayerControl.LocalPlayer || !PlayerControl.LocalPlayer.Data ||
            PlayerControl.LocalPlayer.HasDied())
        {
            return;
        }

        switch (PlayerControl.LocalPlayer.Data.Role)
        {
            case SecretaryRole secr:
                newText =
                    $"\n{TouLocale.GetParsed($"TouJKRoleSecretaryStoredVotes").Replace("<count>", secr.VotesStored.ToString(TownOfUsPlugin.Culture))}";
                break;
        }

        if (newText != string.Empty)
        {
            __instance.TimerText.text += $"<color=#FFFFFF>{newText}</color>";
        }
    }
}