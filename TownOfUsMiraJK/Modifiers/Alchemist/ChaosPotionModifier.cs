using Il2CppSystem.Text;
using System.Text.RegularExpressions;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;
using MiraAPI.Utilities;
using TownOfUs.Modifiers.Game;

namespace TownOfUsMiraJK.Modifiers.Alchemist;

public sealed class ChaosPotionModifier(PlayerControl alchemist) : TimedModifier, IPotionEffect
{
    public override string ModifierName => TouLocale.Get("TouJKRoleAlchemistPotionChaosPotionEffect");
    public bool CanAppear => true;
    public override bool AutoStart => true;
    public override bool HideOnUi => true;
    public override float Duration => 1;
    public PotionType PotionType => PotionType.Neutral;
    public string PotionName => TouLocale.Get("TouJKRoleAlchemistPotionChaosPotion");
    public string PotionDescription => TouLocale.Get("TouJKRoleAlchemistPotionChaosPotionDescription");
    public PlayerControl Alchemist => alchemist;
    public string PotionSymbol => "";
    public override string GetDescription()
    {
        return TouLocale.GetParsed("TouJKRoleAlchemistPotionDescription").Replace("<potion>", PotionName);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        if (PlayerControl.LocalPlayer.IsHost())
        {
            var modifiers = ModifierManager.Modifiers.Where(x => x is GameModifier modifier && x is not AllianceGameModifier && modifier.CanSpawnOnCurrentMode() && modifier.GetAssignmentChance() > 0 && !Player.HasModifier(x.TypeId) && PlayerControl.AllPlayerControls.ToArray().SelectMany(y => y.GetModifiers(x.GetType())).Count() < modifier.GetAmountPerGame() && modifier.IsModifierValidOn(Player.Data.Role));
            if (modifiers.Any())
            {
                modifiers.Shuffle();
                modifiers.Shuffle();
                Player.RpcAddModifier(modifiers.First().GetType());
            }
        }
    }
}