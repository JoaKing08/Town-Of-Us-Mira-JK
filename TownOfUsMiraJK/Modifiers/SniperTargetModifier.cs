using MiraAPI.GameOptions;
using MiraAPI.Roles;
using TownOfUs.Modules.RainbowMod;
using TownOfUsMiraJK.Options.Roles.Impostor;
using UnityEngine;

namespace TownOfUs.Modifiers.Impostor;

public sealed class SniperTargetModifier(PlayerControl owner, Color color, float update)
    : ArrowTargetModifier(owner, color, update)
{
    public override string ModifierName => "Sniper Target";

    public override void OnActivate()
    {
        if (OptionGroupSingleton<SniperOptions>.Instance.ShowArrow && Owner.AmOwner)
        {
            base.OnActivate();
        }

        if (Arrow == null)
        {
            return;
        }

        var spr = Arrow.gameObject.GetComponent<SpriteRenderer>();
        var r = Arrow.gameObject.AddComponent<BasicRainbowBehaviour>();

        r.AddRend(spr, Player.cosmetics.ColorId);
    }

    public override void OnMeetingStart()
    {
        base.OnMeetingStart();
        ModifierComponent!.RemoveModifier(this);
    }
}