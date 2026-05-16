using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs.Events.TouEvents;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Neutral;

namespace TownOfUs.Modifiers.Neutral;

public sealed class BakerFedModifier(byte bakerId) : BaseModifier
{
    public override string ModifierName => "Fed";
    public override bool HideOnUi => true;

    public byte BakerId { get; } = bakerId;
    public byte BreadLeft { get; set; } = (byte)OptionGroupSingleton<BakerOptions>.Instance.BreadLasts;

    public override void OnActivate()
    {
        base.OnActivate();

        var pb = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == BakerId);
        var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.BakerGiveBread, pb!, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }
}