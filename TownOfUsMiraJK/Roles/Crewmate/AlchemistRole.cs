using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Text;
using TMPro;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate.Coroner;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;
using UnityEngine.Events;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class AlchemistRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable, IAssignableTargets
{
    public static int Seed { get; set; }
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string LocaleKey => "Alchemist";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");
    [HideFromIl2Cpp]
    public Type? PotionStored { get; set; }
    public int Priority { get; set; }
    [HideFromIl2Cpp]
    public List<AlchemyIngredient> ChosenIngredients { get; set; } = new();
    public void AssignTargets()
    {
        if (!OptionGroupSingleton<RoleOptions>.Instance.IsClassicRoleAssignment)
        {
            return;
        }
        RpcSetSeed(PlayerControl.LocalPlayer, UnityEngine.Random.RandomRangeInt(int.MinValue, int.MaxValue));
    }

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Brew", "Brew"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}BrewDescription"),
                    ToUJKCrewAssets.AlchemistBrewSprite),

                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Potion", "Potion"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}PotionDescription"),
                    ToUJKCrewAssets.AlchemistPotionSprite)
            };
        }
    }

    public Color RoleColor => TownOfUsMiraJKColors.Alchemist;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = ToUJKRoleIcons.Alchemist,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        IntroSound = TouAudio.ViperIntroSound
    };

    [HideFromIl2Cpp]
    public void BrewPotion(List<AlchemyIngredient> ingredients)
    {
        var seed = Seed;
        foreach (var ingredient in ingredients)
        {
            seed += (int)Math.Pow(2, (int)ingredient);
        }
        var modifiers = ModifierManager.Modifiers.Where(x => x is IPotionEffect { CanAppear: true }).Select(x => x.GetType()).ToArray();
        if (modifiers.Length > 0)
        {
            PotionStored = modifiers[new System.Random(seed).Next(0, modifiers.Length)];
        }

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed($"TouJKRole{LocaleKey}BrewNotif")
            .Replace("<ingredient0>", $"{GetIngredientColor(ingredients[0]).ToTextColor()}{TouLocale.Get($"TouJKRole{LocaleKey}Ingredient{ingredients[0]}")}</color>")
            .Replace("<ingredient1>", $"{GetIngredientColor(ingredients[1]).ToTextColor()}{TouLocale.Get($"TouJKRole{LocaleKey}Ingredient{ingredients[1]}")}</color>")
            .Replace("<potion>", $"{TownOfUsMiraJKColors.Alchemist.ToTextColor()}{(ModifierManager.Modifiers.FirstOrDefault(x => x.GetType() == PotionStored) as IPotionEffect)?.PotionName ?? TouLocale.Get($"TouJKRole{LocaleKey}PotionNone")}</color>"),
            Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Alchemist.LoadAsset());

        notif1.AdjustNotification();
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var text = ITownOfUsRole.SetNewTabText(this);
        text.AppendLine(TouLocale.GetParsed($"TouJKRole{LocaleKey}PotionTab").Replace("<potion>", (ModifierManager.Modifiers.FirstOrDefault(x => x.GetType() == PotionStored) as IPotionEffect)?.PotionName ?? TouLocale.Get($"TouJKRole{LocaleKey}PotionNone")));
        return text;
    }
    [MethodRpc((uint)TownOfUsJKRpc.SetAlchemistSeed)]
    public static void RpcSetSeed(PlayerControl self, int seed)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(self);
            return;
        }
        Seed = seed;
    }
    public static Color GetIngredientColor(AlchemyIngredient ingredient)
    {
        switch (ingredient)
        {
            case AlchemyIngredient.Gold:
                return TownOfUsMiraJKColors.Gold;
            case AlchemyIngredient.Silver:
                return TownOfUsMiraJKColors.Silver;
            case AlchemyIngredient.Copper:
                return TownOfUsMiraJKColors.Copper;
            case AlchemyIngredient.Iron:
                return TownOfUsMiraJKColors.Iron;
            case AlchemyIngredient.Tin:
                return TownOfUsMiraJKColors.Tin;
            case AlchemyIngredient.Mercury:
                return TownOfUsMiraJKColors.Mercury;
            case AlchemyIngredient.Lead:
                return TownOfUsMiraJKColors.Lead;
            default:
                return Color.white;
        }
    }
    public static LoadableAsset<Sprite> GetIngredientIcon(AlchemyIngredient ingredient)
    {
        switch (ingredient)
        {
            case AlchemyIngredient.Gold:
                return ToUJKAssets.Gold;
            case AlchemyIngredient.Silver:
                return ToUJKAssets.Silver;
            case AlchemyIngredient.Copper:
                return ToUJKAssets.Copper;
            case AlchemyIngredient.Iron:
                return ToUJKAssets.Iron;
            case AlchemyIngredient.Tin:
                return ToUJKAssets.Tin;
            case AlchemyIngredient.Mercury:
                return ToUJKAssets.Mercury;
            case AlchemyIngredient.Lead:
                return ToUJKAssets.Lead;
            default:
                return null;
        }
    }

    [MethodRpc((uint)TownOfUsJKRpc.AlchemistNotify)]
    public static void RpcAlchemistNotify(PlayerControl player, PlayerControl source, PlayerControl target)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        if (player.Data.Role is not AlchemistRole)
        {
            Error("RpcAlchemistNotify - Invalid alchemist");
            return;
        }

        if (source.AmOwner || player.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(OptionGroupSingleton<GameMechanicOptions>.Instance.AnonymousShields && !player.AmOwner ? TownOfUsColors.NeutralWiki : TownOfUsMiraJKColors.Alchemist));
        }
    }
}
public enum AlchemyIngredient
{
    Gold,
    Silver,
    Copper,
    Iron,
    Tin,
    Mercury,
    Lead,
}