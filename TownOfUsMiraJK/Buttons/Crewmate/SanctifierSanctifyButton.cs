using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class SanctifierSanctifyButton : TownOfUsRoleButton<SanctifierRole>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleSanctifierSanctify", "Sanctify");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Sanctifier;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<SanctifierOptions>.Instance.SanctifyCooldown + MapCooldown, 5f, 120f);
    public override int MaxUses => (int)OptionGroupSingleton<SanctifierOptions>.Instance.MaxSanctifies;
    public override LoadableAsset<Sprite> Sprite => CrewAssets.SanctifierSanctifySprite;
    public int ExtraUses { get; set; }

    protected override void OnClick()
    {
        var role = PlayerControl.LocalPlayer.GetRole<SanctifierRole>();

        if (role == null)
        {
            return;
        }

        var pos = PlayerControl.LocalPlayer.transform.position;

        SanctifierRole.RpcSanctifierSanctify(PlayerControl.LocalPlayer, pos, pos.z + 0.1f);

        TouAudio.PlaySound(TouAudio.GuardianAngelSound);
    }
}