using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Events.TouEvents;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUs.Modifiers.Impostor;

public sealed class SniperArrowModifier(Vector2 position, Color color) : TimedModifier
{
    private ArrowBehaviour? _arrow;
    public override string ModifierName => "Sniper Arrow";
    public override float Duration => OptionGroupSingleton<SniperOptions>.Instance.ArrowDuration.Value;
    public override bool AutoStart => true;
    public override bool RemoveOnComplete => true;
    public override bool HideOnUi => true;
    public Vector2 Position { get; set; } = position;

    public override void OnActivate()
    {
        base.OnActivate();

        _arrow = MiscUtils.CreateArrow(Player.transform, color);
        _arrow.target = Position;
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        if (!_arrow.IsDestroyedOrNull())
        {
            _arrow?.gameObject.Destroy();
            _arrow?.Destroy();
        }
    }
}