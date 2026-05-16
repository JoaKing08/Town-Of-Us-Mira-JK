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
using TownOfUs.Buttons.Impostor;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Events.TouEvents;
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
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;
using static AmongUs.QuickChat.QuickChatPhraseBuilderResult;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class WitchMarkButton : TownOfUsRoleButton<WitchRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleWitchMark", "Mark");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.Witch;
    public override LoadableAsset<Sprite> Sprite => NeutAssets.WitchMarkSprite;
    public override float Cooldown => 0.001f;
    public override float InitialCooldown => 0.001f;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && !Helpers.GetAlivePlayers().Any(x => x.HasModifier<WitchMarkModifier>(y => y.Witch.PlayerId == Role.Player.PlayerId));
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<WitchMarkModifier>(x => x.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId));
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<WitchMarkModifier>(x => x.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Witch Mark: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<WitchMarkModifier>(x => x.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId);
        players.Do(x => x.RpcRemoveModifier<WitchMarkModifier>());

        Target.RpcAddModifier<WitchMarkModifier>(PlayerControl.LocalPlayer);

        CustomButtonSingleton<WitchControlButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<WitchControlButton>.Instance.ResetCooldownAndOrEffect();
        SetActive(false, Role);
    }
}