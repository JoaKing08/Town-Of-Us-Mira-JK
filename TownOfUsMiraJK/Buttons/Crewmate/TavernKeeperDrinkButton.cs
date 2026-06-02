using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class TavernKeeperDrinkButton : TownOfUsKillRoleButton<TavernKeeperRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleTavernKeeperDrink", "Drink");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.TavernKeeper;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<TavernKeeperOptions>.Instance.DrinkCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => CrewAssets.TavernKeeperDrinkSprite;
    public override int MaxUses => (int)OptionGroupSingleton<TavernKeeperOptions>.Instance.MaxDrinks;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<TavernKeeperDrunkModifier>(x => x.TavernKeeperId == PlayerControl.LocalPlayer.PlayerId));
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Tavern Keeper Drink: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<TavernKeeperDrunkModifier>(x => x.TavernKeeperId == PlayerControl.LocalPlayer.PlayerId);
        players.Do(x => x.RpcRemoveModifier<TavernKeeperDrunkModifier>());

        Target.RpcAddModifier<TavernKeeperDrunkModifier>(PlayerControl.LocalPlayer.PlayerId);
    }
}