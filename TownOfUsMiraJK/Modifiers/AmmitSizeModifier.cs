using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using System.Collections;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUs.Modifiers.Neutral;

public sealed class AmmitSizeModifier() : BaseModifier, IVisualAppearance
{
    public override string ModifierName => "Ammit Size";
    public AmmitRole? Role => Player.GetRole<AmmitRole>();

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        appearance.Size *= 1f + (OptionGroupSingleton<AmmitOptions>.Instance.SizePerPerson * (Role?.Devoured.Count ?? 0) / 100f);
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

    public static IEnumerator CoUpdate(AmmitSizeModifier? modifier)
    {
        if (modifier == null)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.01f);
        modifier.Player.RawSetAppearance(modifier);
    }
}