using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using PowerTools;
using Reactor.Utilities.Extensions;
using TownOfUs.Buttons.Impostor;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modules;
using TownOfUs.Modules.Anims;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class GunslingerAimedModifier(PlayerControl Gunslinger) : BaseModifier
{
    public override string ModifierName => "Aimed";

    public override bool HideOnUi => true;

    public PlayerControl Gunslinger { get; } = Gunslinger;


    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.GunslingerAim, Gunslinger, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void Update()
    {
        if (Player == null || Gunslinger == null)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
        if (Gunslinger.AmOwner)
        {
            var button = CustomButtonSingleton<GunslingerAimButton>.Instance;
            button.UsesLeft += 1;
            button.SetUses(button.UsesLeft);
        }
    }
}