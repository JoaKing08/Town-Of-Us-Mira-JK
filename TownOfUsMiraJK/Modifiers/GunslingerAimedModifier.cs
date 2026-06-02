using MiraAPI.Events;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using TownOfUs.Events.TouEvents;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Enums;

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