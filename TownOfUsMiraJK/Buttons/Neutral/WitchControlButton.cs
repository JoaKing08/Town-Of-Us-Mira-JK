using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class WitchControlButton : TownOfUsRoleButton<WitchRole, PlayerControl>
{
    public PlayerControl Marked => ModifierUtils.GetPlayersWithModifier<WitchMarkModifier>(x => x.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId).FirstOrDefault();
    public override string Name => TouLocale.GetParsed("TouJKRoleWitchControl", "Control");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsMiraJKColors.Witch;
    public override LoadableAsset<Sprite> Sprite => ToUJKNeutAssets.WitchControlSprite;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<WitchOptions>.Instance.ControlCooldown + MapCooldown, 5f, 120f);

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Helpers.GetAlivePlayers().Any(x => x.HasModifier<WitchMarkModifier>(y => y.Witch.PlayerId == Role.Player.PlayerId));
    }

    public override PlayerControl? GetTarget()
    {
        return Marked?.GetClosestLivingPlayer(true, Marked.Data.Role.GetAbilityDistance());
    }

    public override bool CanClick()
    {
        if (TimeLordRewindSystem.IsRewinding)
        {
            return false;
        }

        if (PlayerControl.LocalPlayer.HasDied() && !UsableInDeath)
        {
            return false;
        }

        if (HudManager.Instance.Chat.IsOpenOrOpening || MeetingHud.Instance)
        {
            return false;
        }

        if (!PlayerControl.LocalPlayer.CanMove ||
            PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        if (Timer > 0)
        {
            return false;
        }

        var newTarget = GetTarget();
        if (newTarget != Target)
        {
            SetOutline(false);
        }

        SetOutline(true);

        return PlayerControl.LocalPlayer.moveable;
    }

    protected override void OnClick()
    {
        if (Marked == null)
        {
            Error("Witch Control: Target is null");
            return;
        }

        WitchRole.RpcWitchControl(PlayerControl.LocalPlayer, Marked);
        if (!Marked.HasModifier<WitchRevealModifier>() && OptionGroupSingleton<WitchOptions>.Instance.Learns != WitchLearns.Nothing)
        {
            Marked.AddModifier<WitchRevealModifier>();
        }
    }
}