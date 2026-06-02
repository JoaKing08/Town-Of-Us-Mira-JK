using HarmonyLib;
using TownOfUs;
using TownOfUs.Modules.Localization;
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