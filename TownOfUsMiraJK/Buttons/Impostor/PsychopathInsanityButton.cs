using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers.Neutral;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Impostor;

public sealed class PsychopathInsanityButton : TownOfUsRoleButton<PsychopathRole>
{
    public override string Name => TouLocale.GetParsed("TouJKRolePsychopathInsanity", "Insanity");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Palette.ImpostorRed;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<PsychopathOptions>.Instance.InsanityCooldown + MapCooldown, 5f, 120f);
    public override float EffectDuration => OptionGroupSingleton<PsychopathOptions>.Instance.InsanityDuration;
    public override LoadableAsset<Sprite> Sprite => ToUJKImpAssets.PsychopathInsanitySprite;

    protected override void OnClick()
    {
        if (!EffectActive)
        {
            PlayerControl.LocalPlayer.RpcAddModifier<PsychopathInsanityModifier>();
        }
    }
}