using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.ModifierDisplay;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using System.Collections;
using TownOfUs;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Modifiers;
using TownOfUsMiraJK.Options.Modifiers.Universal;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Game.Universal;

public sealed class DrunkModifier : UniversalGameModifier, IWikiDiscoverable, IColoredModifier
{
    public override string LocaleKey => "Drunk";
    public override string ModifierName => OptionGroupSingleton<DrunkOptions>.Instance.DrunkRandom ? TouLocale.Get($"TouJKModifier{LocaleKey}Random") : TouLocale.Get($"TouJKModifier{LocaleKey}");
    public override string IntroInfo => OptionGroupSingleton<DrunkOptions>.Instance.DrunkRandom ? TouLocale.Get($"TouJKModifier{LocaleKey}IntroBlurbRandom") : TouLocale.Get($"TouJKModifier{LocaleKey}IntroBlurb");
    public override LoadableAsset<Sprite>? ModifierIcon => ToUJKModifierIcons.Drunk;

    public override ModifierFaction FactionType => ModifierFaction.UniversalVisibility;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);
    public int RoundsLeft { get; set; } = OptionGroupSingleton<DrunkOptions>.Instance.DrunkRounds;
    public override bool HideOnUi => !DrunkActive();
    public Color ModifierColor => TownOfUsMiraJKColors.Drunk;
    public int Seed { get; set; }

    public override string GetDescription()
    {
        if (OptionGroupSingleton<DrunkOptions>.Instance.DrunkRandom)
        {
            if (OptionGroupSingleton<DrunkOptions>.Instance.DrunkExpires)
            {
                return TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescriptionExpiresRandom").Replace("<time>", RoundsLeft.ToString());
            }
            else
            {
                return TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescriptionRandom");
            }
        }
        else
        {
            if (OptionGroupSingleton<DrunkOptions>.Instance.DrunkExpires)
            {
                return TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescriptionExpires").Replace("<time>", RoundsLeft.ToString());
            }
            else
            {
                return TouLocale.GetParsed($"TouJKModifier{LocaleKey}TabDescription");
            }
        }
    }

    public string GetAdvancedDescription()
    {
        return TouLocale.GetParsed($"TouJKModifier{LocaleKey}WikiDescription") + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<UniversalModifierJKOptions>.Instance.DrunkChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<UniversalModifierJKOptions>.Instance.DrunkAmount;
    }

    public bool DrunkActive()
    {
        return !OptionGroupSingleton<DrunkOptions>.Instance.DrunkExpires || RoundsLeft > 0;
    }

    public override void OnActivate()
    {
        if (PlayerControl.LocalPlayer.IsHost() && OptionGroupSingleton<DrunkOptions>.Instance.DrunkRandom)
        {
            RpcSetDrunkSeed(Player, UnityEngine.Random.RandomRangeInt(int.MinValue, int.MaxValue));
        }
    }

    public static IEnumerator CoSetDrunkSeed(PlayerControl drunk, int seed)
    {
        yield return new WaitForSeconds(0.01f);
        if (!drunk.TryGetModifier<DrunkModifier>(out var modifier))
        {
            yield break;
        }
        modifier.Seed = seed;
        List<char> list = ['D', 'A', 'S', 'W'];
        list.Shuffle(seed);
        Message($"Set Drunk seed to {seed} [D -> {list[0]}, A -> {list[1]}, S -> {list[2]}, W -> {list[3]}]");
    }

    [HarmonyPatch]
    public static class PlayerPhysicsPatch
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        [HarmonyPostfix]
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.TryGetModifier<DrunkModifier>(out var drunk) && __instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove && drunk.DrunkActive())
            {
                if (OptionGroupSingleton<DrunkOptions>.Instance.DrunkRandom)
                {
                    List<float> toTransform = [Mathf.Max(__instance.body.velocity.x, 0f), Mathf.Max(-__instance.body.velocity.x, 0f), Mathf.Max(__instance.body.velocity.y, 0f), Mathf.Max(-__instance.body.velocity.y, 0f)];
                    toTransform.Shuffle(drunk.Seed);
                    __instance.body.velocity = new Vector2(toTransform[0] - toTransform[1], toTransform[2] - toTransform[3]);
                }
                else
                {
                    __instance.body.velocity *= -1f;
                }
            }
        }
    }

    public override void OnMeetingStart()
    {
        RoundsLeft--;
        if (Player.AmOwner)
        {
            ModifierDisplayComponent.Instance?.RefreshModifiers();
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.SetDrunkSeed, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcSetDrunkSeed(PlayerControl drunk, int seed)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(drunk);
            return;
        }
        Coroutines.Start(CoSetDrunkSeed(drunk, seed));
    }
}