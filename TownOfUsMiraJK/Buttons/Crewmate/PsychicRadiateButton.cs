using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class PsychicRadiateButton : TownOfUsRoleButton<PsychicRole>
{
    public override string Name => TouLocale.GetParsed("TouJKRolePsychicRadiate", "Radiate");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Psychic;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<PsychicOptions>.Instance.RadiateCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => CrewAssets.PsychicRadiateSprite;
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && OptionGroupSingleton<PsychicOptions>.Instance.RadiateCount > 0;
    }
    public override bool CanUse()
    {
        return base.CanUse() && Helpers.GetClosestPlayers(PlayerControl.LocalPlayer, OptionGroupSingleton<PsychicOptions>.Instance.RadiateRange * ShipStatus.Instance.MaxLightRadius).Count != 0;
    }

    protected override void OnClick()
    {
        var role = PlayerControl.LocalPlayer.GetRole<PsychicRole>();

        if (role == null)
        {
            return;
        }

        foreach (var player in Helpers.GetClosestPlayers(PlayerControl.LocalPlayer, OptionGroupSingleton<PsychicOptions>.Instance.RadiateRange * ShipStatus.Instance.MaxLightRadius))
        {
            if (player.TryGetModifier<PsychicColoredModifier>(out var modifier) && UnityEngine.Random.RandomRangeInt(1, 101) <= OptionGroupSingleton<PsychicOptions>.Instance.RadiateSucceedChance)
            {
                modifier.Charges++;
            }
        }

        TouAudio.PlaySound(TouAudio.QuestionSound);
    }
}