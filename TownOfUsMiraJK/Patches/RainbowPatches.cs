using AmongUs.Data;
using HarmonyLib;
using TownOfUs;
using TownOfUs.Modules.RainbowMod;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modules.RainbowMod;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace TownOfUsMiraJK.RainbowMod;

[HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(int), typeof(Renderer))]
[HarmonyAfter(TownOfUsPlugin.Id)]
public static class SetPlayerMaterialPatch
{
    public static bool Prefix([HarmonyArgument(0)] int colorId, [HarmonyArgument(1)] Renderer rend)
    {
        return !RainbowJKUtils.IsGrayscale(colorId) && !RainbowJKUtils.IsFire(colorId) && !RainbowJKUtils.IsGalaxy(colorId);
    }
}

[HarmonyPatch(typeof(PlayerTab))]
[HarmonyAfter(TownOfUsPlugin.Id)]
public static class PlayerTabPatch
{
    [HarmonyPatch(nameof(PlayerTab.Update))]
    [HarmonyPostfix]
    public static void UpdatePostfix(PlayerTab __instance)
    {
        for (var i = 0; i < __instance.ColorChips.Count; i++)
        {
            if (RainbowJKUtils.IsGrayscale(i))
            {
                __instance.ColorChips[i].Inner.SpriteColor = RainbowJKUtils.Grayscale;
            }
            else if (RainbowJKUtils.IsFire(i))
            {
                __instance.ColorChips[i].Inner.SpriteColor = RainbowJKUtils.Fire;
            }
            else if (RainbowJKUtils.IsGalaxy(i))
            {
                __instance.ColorChips[i].Inner.SpriteColor = RainbowJKUtils.Galaxy;
            }
        }
    }
}

[HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.Update))]
[HarmonyAfter(TownOfUsPlugin.Id)]
public static class ChatNotifRainbowPatch
{
    public static void Prefix(ChatNotification __instance)
    {
        if (__instance.gameObject.active)
        {
            if (RainbowJKUtils.IsGrayscale(__instance.player.cosmetics.ColorId))
            {
                string str = ColorUtility.ToHtmlStringRGB(RainbowJKUtils.SetBasicGrayscale());
                __instance.playerNameText.text = "<color=#" + str + ">" + __instance.playerNameText.text.WithoutRichText();
            }
            else if (RainbowJKUtils.IsFire(__instance.player.cosmetics.ColorId))
            {
                string str = ColorUtility.ToHtmlStringRGB(RainbowJKUtils.SetBasicFire());
                __instance.playerNameText.text = "<color=#" + str + ">" + __instance.playerNameText.text.WithoutRichText();
            }
            else if (RainbowJKUtils.IsGalaxy(__instance.player.cosmetics.ColorId))
            {
                string str = ColorUtility.ToHtmlStringRGB(RainbowJKUtils.SetBasicGalaxy());
                __instance.playerNameText.text = "<color=#" + str + ">" + __instance.playerNameText.text.WithoutRichText();
            }
        }
    }

    public static void Postfix(ChatNotification __instance)
    {
        if (__instance.gameObject.active)
        {
            if (RainbowJKUtils.IsGrayscale(__instance.player.cosmetics.ColorId))
            {
                string str = ColorUtility.ToHtmlStringRGB(RainbowJKUtils.SetBasicGrayscale());
                __instance.playerNameText.text = "<color=#" + str + ">" + __instance.playerNameText.text.WithoutRichText();
            }
            else if (RainbowJKUtils.IsFire(__instance.player.cosmetics.ColorId))
            {
                string str = ColorUtility.ToHtmlStringRGB(RainbowJKUtils.SetBasicFire());
                __instance.playerNameText.text = "<color=#" + str + ">" + __instance.playerNameText.text.WithoutRichText();
            }
            else if (RainbowJKUtils.IsGalaxy(__instance.player.cosmetics.ColorId))
            {
                string str = ColorUtility.ToHtmlStringRGB(RainbowJKUtils.SetBasicGalaxy());
                __instance.playerNameText.text = "<color=#" + str + ">" + __instance.playerNameText.text.WithoutRichText();
            }
        }
    }
}

[HarmonyPatch(typeof(HostInfoPanel), nameof(HostInfoPanel.Update))]
[HarmonyAfter(TownOfUsPlugin.Id)]
public static class RainbowLobbyInfoPanePatch
{
    public static void Postfix(HostInfoPanel __instance)
    {
        if (__instance.gameObject.activeInHierarchy && (RainbowJKUtils.IsGrayscale(__instance.player.cosmetics.ColorId) || RainbowJKUtils.IsFire(__instance.player.cosmetics.ColorId) || RainbowJKUtils.IsGalaxy(__instance.player.cosmetics.ColorId)))
        {
            NetworkedPlayerInfo host = GameData.Instance.GetHost();
            string text = "";

            if (RainbowJKUtils.IsGrayscale(__instance.player.cosmetics.ColorId))
            {
                text = ColorUtility.ToHtmlStringRGB(RainbowJKUtils.SetBasicGrayscale());
            }

            if (RainbowJKUtils.IsFire(__instance.player.cosmetics.ColorId))
            {
                text = ColorUtility.ToHtmlStringRGB(RainbowJKUtils.SetBasicFire());
            }

            if (RainbowJKUtils.IsGalaxy(__instance.player.cosmetics.ColorId))
            {
                text = ColorUtility.ToHtmlStringRGB(RainbowJKUtils.SetBasicGalaxy());
            }

            __instance.hostLabel.text =
                TranslationController.Instance.GetString(StringNames.HostNounLabel,
                    Array.Empty<Object>());
            if (__instance.ShouldBoldenHostLabel(DataManager.Settings.Language.CurrentLanguage))
            {
                __instance.hostLabel.text = __instance.hostLabel.text.Insert(0, "<b>");
                __instance.hostLabel.text = __instance.hostLabel.text.Insert(__instance.hostLabel.text.Length, "</b>");
            }

            if (AmongUsClient.Instance.AmHost)
            {
                __instance.playerName.text = (string.IsNullOrEmpty(host.PlayerName)
                                                 ? "..."
                                                 : $"<color=#{text}>{host.PlayerName}</color>")
                                             + "  <size=90%><b><font=\"Barlow-BoldItalic SDF\" material=\"Barlow-BoldItalic SDF Outline\">" +
                                             TranslationController.Instance.GetString(
                                                 StringNames.HostYouLabel, Array.Empty<Object>());
            }
            else
            {
                __instance.playerName.text =
                    (string.IsNullOrEmpty(host.PlayerName) ? "..." : $"<color=#{text}>{host.PlayerName}</color>") +
                    " (" + __instance.player.ColorBlindName + ")";
            }
        }
    }
}