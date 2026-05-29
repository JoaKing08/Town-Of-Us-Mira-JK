using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Networking;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modules;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;
using static AmongUs.QuickChat.QuickChatPhraseBuilderResult;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class WitchControlButton : TownOfUsRoleButton<WitchRole, PlayerControl>
{
    public PlayerControl Marked => ModifierUtils.GetPlayersWithModifier<WitchMarkModifier>(x => x.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId).FirstOrDefault();
    public override string Name => TouLocale.GetParsed("TouJKRoleWitchControl", "Control");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Witch;
    public override LoadableAsset<Sprite> Sprite => NeutAssets.WitchControlSprite;
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
    }
}