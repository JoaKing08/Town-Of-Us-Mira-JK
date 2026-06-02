using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Networking;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class AnarchistAssaultButton : TownOfUsButton
{
    public override string Name => TouLocale.GetParsed("TouJKRoleAnarchistAssault", "Assault");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => Colors.Anarchist;
    public override float Cooldown => 0.01f;
    public override LoadableAsset<Sprite> Sprite => NeutAssets.AnarchistAssaultSprite;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override bool ShouldPauseInVent => false;
    public override bool UsableInDeath => true;
    public bool Show { get; set; }

    public override bool Enabled(RoleBehaviour? role)
    {
        return Show && ModifierUtils.GetActiveModifiers<MisfortuneTargetModifier>().HasAny();
    }

    protected override void OnClick()
    {
        if (Minigame.Instance)
        {
            return;
        }

        var playerMenu = CustomPlayerMenu.Create();
        playerMenu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.Begin(
            plr => !plr.HasDied() && plr.HasModifier<MisfortuneTargetModifier>() &&
                   !plr.HasModifier<InvulnerabilityModifier>() && !plr.AmOwner,
            plr =>
            {
                playerMenu.ForceClose();

                if (plr != null && ModifierUtils.GetActiveModifiers<MisfortuneTargetModifier>().HasAny())
                {
                    PlayerControl.LocalPlayer.RpcGhostRoleMurder(plr);
                    foreach (var mod in ModifierUtils.GetActiveModifiers<MisfortuneTargetModifier>())
                    {
                        mod.ModifierComponent?.RemoveModifier(mod);
                    }

                    Show = false;
                }
            });
    }

    public override bool CanUse()
    {
        if (HudManager.Instance.Chat.IsOpenOrOpening || MeetingHud.Instance)
        {
            return false;
        }

        return ModifierUtils.GetActiveModifiers<MisfortuneTargetModifier>().HasAny();
    }
}