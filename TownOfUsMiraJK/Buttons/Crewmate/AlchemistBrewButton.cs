using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Roles;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class AlchemistBrewButton : TownOfUsRoleButton<AlchemistRole>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleAlchemistBrew", "Brew");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsMiraJKColors.Alchemist;
    public override float Cooldown => OptionGroupSingleton<AlchemistOptions>.Instance.BrewCooldown;
    public override LoadableAsset<Sprite> Sprite => ToUJKCrewAssets.AlchemistBrewSprite;
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && (role as AlchemistRole)?.PotionStored == null;
    }

    protected override void OnClick()
    {
        if (Role.ChosenIngredients.Count == 0)
        {
            var allIngredients = Enum.GetValues<AlchemyIngredient>().ToList();
            allIngredients.Shuffle();
            allIngredients.Shuffle();
            Role.ChosenIngredients.AddRange(allIngredients.GetRange(0, (int)OptionGroupSingleton<AlchemistOptions>.Instance.IngredientChoices));
        }
        if (!Minigame.Instance)
        {
            var alchemistMenu = AlchemistBrewMinigame.Create();
            alchemistMenu.Open(
                Role.ChosenIngredients,
                (ingredient, id, card) =>
                {
                    if (alchemistMenu.ChosenCards.Contains((id, ingredient)))
                    {
                        alchemistMenu.ChosenCards.Remove((id, ingredient));
                        card.gameObject.GetComponent<ButtonRolloverHandler>().OverColor = AlchemistRole.GetIngredientColor(ingredient);
                        card.gameObject.GetComponent<ButtonRolloverHandler>().OutColor = TownOfUsMiraJKColors.Alchemist;
                        card.gameObject.GetComponent<SpriteRenderer>().color = AlchemistRole.GetIngredientColor(ingredient);
                    }
                    else
                    {
                        alchemistMenu.ChosenCards.Add((id, ingredient));
                        card.gameObject.GetComponent<ButtonRolloverHandler>().OverColor = Color.Lerp(AlchemistRole.GetIngredientColor(ingredient), Color.white, 0.5f);
                        card.gameObject.GetComponent<ButtonRolloverHandler>().OutColor = Color.Lerp(TownOfUsMiraJKColors.Alchemist, Color.white, 0.5f);
                        card.gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(AlchemistRole.GetIngredientColor(ingredient), Color.white, 0.5f);
                    }
                    if (alchemistMenu.ChosenCards.Count == 2)
                    {
                        Role.BrewPotion(alchemistMenu.ChosenCards.Select(x => x.Item2).ToList());
                        Role.ChosenIngredients.Clear();
                        alchemistMenu.Close();
                        SetActive(false, Role);
                        CustomButtonSingleton<AlchemistPotionButton>.Instance.SetActive(true, Role);
                    }
                }
            );
        }
    }
}