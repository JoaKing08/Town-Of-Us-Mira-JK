using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Alchemist;

public sealed class VisibleToClaivoyantModifier() : BaseModifier
{
    public override string ModifierName => "Visible To Clairvoyant";
    public override bool HideOnUi => true;

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        if (!Player.Data.IsDead || !PlayerControl.LocalPlayer.HasModifier<ClairvoyantPotionModifier>())
        {
            ModifierComponent?.RemoveModifier(this);
        }
        if (PlayerControl.LocalPlayer.HasModifier<ClairvoyantPotionModifier>())
        {
            Player.Visible = true;
        }
        base.FixedUpdate();
    }
}