using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class ShadowDarknessButton : TownOfUsRoleButton<ShadowRole>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleShadowDarkness", "Darkness");
    public override BaseKeybind Keybind => Keybinds.TertiaryAction;
    public override Color TextOutlineColor => Colors.Shadow;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ShadowOptions>.Instance.DarknessCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<ShadowOptions>.Instance.DarknessDuration;
    public override LoadableAsset<Sprite> Sprite => NeutAssets.ShadowDarknessSprite;

    public override bool ZeroIsInfinite { get; set; } = true;

    protected override void OnClick()
    {
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.AmOwner))
        {
            player.RpcAddModifier<ShadowDarknessModifier>(PlayerControl.LocalPlayer);
        }
    }
}