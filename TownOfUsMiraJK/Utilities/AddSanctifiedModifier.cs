using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TownOfUs;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Utilities
{
    [HarmonyPatch]
    public static class AddSanctifierModifier
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.AmOwner)
            {
                return;
            }
            if (!PlayerControl.LocalPlayer.IsHost() && !TutorialManager.Instance)
            {
                return;
            }
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.Disconnected))
            {
                if (!player.HasDied() && SanctifierCircle.IsInCircle(player.transform))
                {
                    if (!player.HasModifier<SanctifiedModifier>())
                    {
                        player.RpcAddModifier<SanctifiedModifier>();
                    }
                }
                else
                {
                    if (player.HasModifier<SanctifiedModifier>())
                    {
                        player.RpcRemoveModifier<SanctifiedModifier>();
                    }
                }
            }
        }
    }
}
