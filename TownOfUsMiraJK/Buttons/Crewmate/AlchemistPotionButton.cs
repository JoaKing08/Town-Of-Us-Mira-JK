using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Buttons.Crewmate;

public sealed class AlchemistPotionButton : TownOfUsRoleButton<AlchemistRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleAlchemistPotion", "Potion");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsMiraJKColors.Alchemist;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<AlchemistOptions>.Instance.PotionCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<AlchemistOptions>.Instance.PotionDelay;
    public override LoadableAsset<Sprite> Sprite => ToUJKCrewAssets.AlchemistPotionSprite;
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && (role as AlchemistRole)?.PotionStored != null;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance) ?? ((ModifierManager.Modifiers.FirstOrDefault(x => x.GetType() == Role?.PotionStored) as IPotionEffect)?.CanSelf == true ? PlayerControl.LocalPlayer : null);
    }
    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && Role?.PotionStored != null && target?.HasModifier(Role.PotionStored) == false && !target.HasModifier<AlchemistPotionModifier>(x => x.ToApply == Role.PotionStored);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error($"{Name}: Target is null");
            return;
        }
        if (Role?.PotionStored == null)
        {
            Error($"{Name}: Potion is null");
            return;
        }

        Target?.RpcAddModifier<AlchemistPotionModifier>(PlayerControl.LocalPlayer.PlayerId, ModifierManager.GetModifierTypeId(Role.PotionStored) ?? 0);
        OverrideName(TouLocale.GetParsed("TouJKRoleAlchemistPotionApplying", "Applying"));
    }
    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        OverrideName(TouLocale.GetParsed("TouJKRoleAlchemistPotion", "Potion"));
        Role.PotionStored = null;
        SetActive(false, Role);
        CustomButtonSingleton<AlchemistBrewButton>.Instance.SetActive(true, Role);
    }
}