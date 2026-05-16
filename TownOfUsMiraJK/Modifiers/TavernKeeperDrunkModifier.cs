using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using TownOfUs.Events.TouEvents;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;

namespace TownOfUs.Modifiers.Neutral;

public sealed class TavernKeeperDrunkModifier(byte tavernKeeperId) : TimedModifier
{
    public override string ModifierName => "Drunk";

    public byte TavernKeeperId { get; } = tavernKeeperId;
    public override float Duration => OptionGroupSingleton<TavernKeeperOptions>.Instance.DrinkDuration;
    public override bool RemoveOnComplete => true;

    public override void OnActivate()
    {
        base.OnActivate();

        var pb = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == TavernKeeperId);
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.TavernKeeperDrink, pb!, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        StartTimer();
    }
}