using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Buttons.Impostor;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game.Alliance;
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
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;
using static TownOfUs.Patches.Options.TeamChatPatches;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUs.Patches.Roles;

[HarmonyPatch]
public static class JKIntroScenePatches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static bool ImpostorBeginPatch(IntroCutscene __instance)
    {
        __instance.TeamTitle.text =
            TranslationController.Instance.GetString(StringNames.Impostor, Array.Empty<Il2CppSystem.Object>());
        __instance.TeamTitle.color = Palette.ImpostorRed;

        var player = __instance.CreatePlayer(0, 1, PlayerControl.LocalPlayer.Data, true);
        __instance.ourCrewmate = player;

        if (!OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode)
        {
            var i = 1;
            foreach (var impostor in PlayerControl.AllPlayerControls.ToArray().Where(x => !x.AmOwner && (x.IsImpostor() || x.Is((RoleTypes)RoleId.Get<UndercoverRole>()))))
            {
                __instance.CreatePlayer(i++, 1, impostor.Data, true);
            }
            var crewpostor = ModifierUtils.GetPlayersWithModifier<CrewpostorModifier>().FirstOrDefault();
            if (crewpostor != null && !OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode && !crewpostor.AmOwner)
            {
                __instance.CreatePlayer(i++, 1, crewpostor.Data, true);
            }
            IntroScenePatches.SetHiddenImpostors(__instance);
        }

        return false;
    }
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static bool CrewmateBeginPatch(IntroCutscene __instance, [HarmonyArgument(0)] ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        if (PlayerControl.LocalPlayer.HasModifier<CrewpostorModifier>())
        {
            ImpostorBeginPatch(__instance);
            return false;
        }
        if (PlayerControl.LocalPlayer.IsApocalypseAligned())
        {
            var apocTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            apocTeam.Add(PlayerControl.LocalPlayer);
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x?.IsApocalypseAligned() == true && !x.AmOwner))
            {
                apocTeam.Add(player);
            }

            yourTeam = apocTeam;
            return true;
        }

        return true;
    }
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    [HarmonyPriority(Priority.Last)]
    [HarmonyPrefix]
    public static void RecruitBeginPatch(IntroCutscene __instance, [HarmonyArgument(0)] ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
    }
}