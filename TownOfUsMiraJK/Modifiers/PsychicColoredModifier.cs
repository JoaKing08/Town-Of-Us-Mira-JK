using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using System.Collections;
using TownOfUs.Events.TouEvents;
using TownOfUs.Options.Modifiers.Universal;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUs.Modifiers.Neutral;

public sealed class PsychicColoredModifier(PlayerControl psychic, Color color, byte colorId, string roleName = "") : ConcealedModifier, IVisualAppearance
{
    public override string ModifierName => "Psychic Colored";
    public PlayerControl Psychic { get; } = psychic;
    public Color Color { get; set; } = color;
    public byte ColorId { get; set; } = colorId;
    public string RoleName { get; set; } = roleName;
    public override bool VisibleToOthers => true;
    public byte Charges { get; set; }

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = new VisualAppearance(Player.GetDefaultModifiedAppearance(), TownOfUsAppearances.Swooper)
        {
            ColorId = ColorId,
            HatId = "hat_NoHat",
            SkinId = "skin_None",
            VisorId = "visor_EmptyVisor",
            PlayerName = RoleName,
            PetId = "pet_EmptyPet",
            RendererColor = Color,
            NameColor = Color,
        };
        if (Color == Color.clear)
        {
            appearance.ColorBlindTextColor = Color;
        }
        return appearance;
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);
    }

    public override void OnDeactivate()
    {
        Player?.ResetAppearance(fullReset: true);
    }

    public void UpdateData(Color? color = null, byte? colorId = null, string? roleName = null)
    {
        if ((color != null && color != Color) || (colorId != null && colorId != ColorId) || (roleName != null && roleName != RoleName))
        {
            if (color != null)
            {
                Color = (Color)color;
            }
            if (colorId != null)
            {
                ColorId = (byte)colorId;
            }
            if (roleName != null)
            {
                RoleName = roleName;
            }
            Player.RawSetAppearance(this);
        }
    }
    public override void OnMeetingStart()
    {
        if (Psychic.HasDied())
        {
            ModifierComponent?.RemoveModifier(this);
        }
    }
}