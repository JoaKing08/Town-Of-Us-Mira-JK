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

public sealed class ExecutorAimedModifier(PlayerControl executor) : BaseModifier
{
    public override string ModifierName => "Aimed";

    public override bool HideOnUi => true;

    public PlayerControl Executor { get; } = executor;


    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.ExecutorAim, Executor, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void Update()
    {
        if (Player == null || Executor == null)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
        if (Executor.AmOwner)
        {
            var button = CustomButtonSingleton<ExecutorAimButton>.Instance;
            button.UsesLeft += 1;
            button.SetUses(button.UsesLeft);
        }
    }
}