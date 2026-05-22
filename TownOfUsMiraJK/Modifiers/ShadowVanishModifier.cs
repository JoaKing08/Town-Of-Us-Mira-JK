using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using TownOfUs.Assets;
using TownOfUs.Buttons.Impostor;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Patches;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Secret;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Secret;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Secret;

public sealed class ShadowVanishModifier : ConcealedModifier, IVisualAppearance
{
    public override string ModifierName => "Vanished";
    public override float Duration => OptionGroupSingleton<ShadowOptions>.Instance.VanishDuration;
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public override bool VisibleToOthers => false;
    public bool VisualPriority => true;

    public VisualAppearance GetVisualAppearance()
    {
        var opacity = OptionGroupSingleton<ShadowOptions>.Instance.VanishOpacity.Value;
        var playerColor = Colors.Shadow.SetAlpha(Player.AmOwner || (PlayerControl.LocalPlayer.DiedOtherRound() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow) ? MathF.Max(0.1f, opacity / 100f) : opacity / 100f);

        var visualAppearance = new VisualAppearance(Player.GetDefaultModifiedAppearance(), TownOfUsAppearances.Swooper)
        {
            HatId = "hat_NoHat",
            SkinId = "skin_None",
            VisorId = "visor_EmptyVisor",
            PlayerName = string.Empty,
            PetId = "pet_EmptyPet",
            RendererColor = playerColor,
            NameColor = Color.clear,
            ColorBlindTextColor = Color.clear
        };
        visualAppearance.Speed = OptionGroupSingleton<ShadowOptions>.Instance.VanishSpeed;
        return visualAppearance;
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }

    public override void OnActivate()
    {
        if (Player.AmOwner)
        {
            TouAudio.PlaySound(TouAudio.SwooperActivateSound);
        }

        Player.RawSetAppearance(this);
        Player.cosmetics.ToggleNameVisible(false);

        var button = CustomButtonSingleton<ShadowVanishButton>.Instance;
        button.OverrideSprite(SecrAssets.ShadowAppearSprite.LoadAsset());
        button.OverrideName(TouLocale.Get("TouJKRoleShadowAppear", "Appear"));

        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.ShadowVanish, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (VanillaSystemCheckPatches.ShroomSabotageSystem && VanillaSystemCheckPatches.ShroomSabotageSystem.IsActive)
        {
            Player.RawSetAppearance(this);
            Player.cosmetics.ToggleNameVisible(false);
        }
    }

    public override void OnDeactivate()
    {
        Player.ResetAppearance();
        Player.cosmetics.ToggleNameVisible(true);

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<ShadowVanishButton>.Instance;
            button.OverrideSprite(SecrAssets.ShadowVanishSprite.LoadAsset());
            button.OverrideName(TouLocale.Get("TouJKRoleShadowVanish", "Vanish"));
            if (!MeetingHud.Instance)
            {
                TouAudio.PlaySound(TouAudio.SwooperDeactivateSound);
            }
        }

        if (HudManagerPatches.CamouflageCommsEnabled)
        {
            Player.cosmetics.ToggleNameVisible(false);
        }

        if (VanillaSystemCheckPatches.ShroomSabotageSystem && VanillaSystemCheckPatches.ShroomSabotageSystem.IsActive)
        {
            MushroomMixUp(VanillaSystemCheckPatches.ShroomSabotageSystem, Player);
        }

        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.ShadowAppear, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public static void MushroomMixUp(MushroomMixupSabotageSystem instance, PlayerControl player)
    {
        if (player != null && !player.Data.IsDead && instance.currentMixups.ContainsKey(player.PlayerId))
        {
            var condensedOutfit = instance.currentMixups[player.PlayerId];
            var playerOutfit = instance.ConvertToPlayerOutfit(condensedOutfit);
            playerOutfit.NamePlateId = player.Data.DefaultOutfit.NamePlateId;

            player.MixUpOutfit(playerOutfit);
        }
    }
}