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
    public Type? PotionStored
    {
        get
        {
            return _potionStored;
        }
        set
        {
            _potionStored = value;
            var potion = ModifierManager.Modifiers.FirstOrDefault(x => x.GetType() == value) as IPotionEffect;
            if (potion == null)
            {
                PotionStoredName = TouLocale.Get($"TouJKRole{LocaleKey}PotionNone");
                PotionStoredCanSelf = false;
                PotionStoredType = PotionType.None;
            }
            else
            {
                PotionStoredName = potion.PotionName;
                PotionStoredCanSelf = potion.CanSelf;
                PotionStoredType = potion.PotionType;
            }
        }
    }
    private Type? _potionStored;
    public string PotionStoredName { get; private set; } = TouLocale.Get($"TouJKRoleAlchemistPotionNone");
    public bool PotionStoredCanSelf { get; private set; }
    public PotionType PotionStoredType { get; private set; } = PotionType.None;
    public int Priority { get; set; }
    public List<RecordedCombination> KnownCombinations { get; set; } = new();
    public RecordedCombination LastCombination { get; set; }
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
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}BrewWikiDescription"),
                    ToUJKCrewAssets.AlchemistBrewSprite),

                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Potion", "Potion"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}PotionWikiDescription"),
                    ToUJKCrewAssets.AlchemistPotionSprite),

                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}PositivePotions"),
                    PositivePotionList,
                    TouRoleIcons.Cleric),

                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}NeutralPotions"),
                    NeutralPotionList,
                    TouRoleIcons.Aurial),

                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}NegativePotions"),
                    NegativePotionList,
                    ToUJKRoleIcons.Poisoner),

            };
        }
    }

    public static string PositivePotionList
    {
        get
        {
            if (_positivePotionList == null)
            {
                _positivePotionList = GetPotionList(PotionType.Positive);
            }
            return _positivePotionList;
        }
    }
    private static string _positivePotionList;

    public static string NeutralPotionList
    {
        get
        {
            if (_neutralPotionList == null)
            {
                _neutralPotionList = GetPotionList(PotionType.Neutral);
            }
            return _neutralPotionList;
        }
    }
    private static string _neutralPotionList;

    public static string NegativePotionList
    {
        get
        {
            if (_negativePotionList == null)
            {
                _negativePotionList = GetPotionList(PotionType.Negative);
            }
            return _negativePotionList;
        }
    }
    private static string _negativePotionList;

    public static string GetPotionList(PotionType type)
    {
        var text = new StringBuilder();
        foreach (var modifier in ModifierManager.Modifiers.Where(x => x is IPotionEffect potion && potion.PotionType == type))
        {
            var potion = modifier as IPotionEffect;
            text.AppendLine($"{potion!.PotionName}{(string.IsNullOrWhiteSpace(potion.PotionSymbol) ? "" : $" ({TownOfUsMiraJKColors.Alchemist.ToTextColor()}{potion.PotionSymbol}</color>)")}{(string.IsNullOrWhiteSpace(potion.PotionDescription) ? "" : $": {potion.PotionDescription}")}");
        }
        text = text.Remove(text.Length - 1, 1);
        return text.ToString();
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

        LastCombination = KnownCombinations.Any(x => x.Is(ingredients)) ? KnownCombinations.First(x => x.Is(ingredients)) : new(ingredients, OptionGroupSingleton<AlchemistOptions>.Instance.LearnPotion ? PotionStoredName : TouLocale.GetParsed($"TouJKRole{LocaleKey}Potion{PotionStoredType}"));
        if (!KnownCombinations.Contains(LastCombination))
        {
            KnownCombinations.Add(LastCombination);
        }

        var notif1 = Helpers.CreateAndShowNotification(
            TouLocale.GetParsed($"TouJKRole{LocaleKey}BrewNotif")
            .Replace("<ingredient0>", $"{GetIngredientColor(ingredients[0]).ToTextColor()}{TouLocale.Get($"TouJKRole{LocaleKey}Ingredient{ingredients[0]}")}</color>")
            .Replace("<ingredient1>", $"{GetIngredientColor(ingredients[1]).ToTextColor()}{TouLocale.Get($"TouJKRole{LocaleKey}Ingredient{ingredients[1]}")}</color>")
            .Replace("<potion>", $"{TownOfUsMiraJKColors.Alchemist.ToTextColor()}{LastCombination.Name}</color>"),
            Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Alchemist.LoadAsset());

        notif1.AdjustNotification();
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
public struct RecordedCombination
{
    public List<AlchemyIngredient> Ingredients = new();
    public string Name = "";
    public RecordedCombination(List<AlchemyIngredient> ingredients, string name)
    {
        Ingredients = ingredients.OrderBy(x => x).ToList();
        Name = name;
    }
    public bool Is(List<AlchemyIngredient> ingredients)
    {
        ingredients = ingredients.OrderBy(x => x).ToList();
        var result = Ingredients.Count == ingredients.Count;
        if (result)
        {
            for (int i = 0; i < ingredients.Count; i++)
            {
                result &= Ingredients[i] == ingredients[i];
            }
        }
        return result;
    }
    public bool Is(params AlchemyIngredient[] ingredients)
    {
        return Is(ingredients.ToList());
    }
}