using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using TownOfUs.Buttons;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Networking;
using TownOfUs.Options;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Events.Crewmate;

public static class BodyguardEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var target = @event.Target;

        if (target.Data.Role is BodyguardRole)
        {
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x.HasModifier<BodyguardGuardModifier>(y => y.Bodyguard == target)))
            {
                player.RemoveModifier<BodyguardGuardModifier>(x => x.Bodyguard == target);
            }
        }
    }
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;

        if (target == null || button == null || button is not IKillButton || !button.CanClick())
        {
            return;
        }

        CheckForBodyguardGuard(@event, target, PlayerControl.LocalPlayer);
    }

    [RegisterEvent]
    public static void MiraButtonCancelledEventHandler(MiraButtonCancelledEvent @event)
    {
        var source = PlayerControl.LocalPlayer;
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;

        if (target && !target!.HasModifier<BodyguardGuardModifier>() && button != null && button is IKillButton)
        {
            return;
        }

        ResetButtonTimer(source, button);
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (CheckForBodyguardGuard(@event, target, source))
        {
            ResetButtonTimer(source);
        }
    }

    private static bool CheckForBodyguardGuard(MiraCancelableEvent @event, PlayerControl target,
        PlayerControl source)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return false;
        }

        if (!target.HasModifier<BodyguardGuardModifier>() ||
            target.PlayerId == source.PlayerId ||
            source.TryGetModifier<IndirectAttackerModifier>(out var indirect))
        {
            return false;
        }

        MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Error, $"{target.Data.PlayerName} has a bodyguard guard, stopping an attack from {source.Data.PlayerName}!");
        @event.Cancel();

        var bodyguard = target.GetModifier<BodyguardGuardModifier>()?.Bodyguard.GetRole<BodyguardRole>();

        if (bodyguard != null && (TutorialManager.InstanceExists || source.AmOwner))
        {
            BodyguardRole.RpcTeleportBodyguard(bodyguard.Player, target.PlayerId, source.PlayerId);
            bodyguard.Player.RpcSpecialMurder(source, teleportMurderer: false, causeOfDeath: "Bodyguard");
            source.RpcSpecialMurder(bodyguard.Player, teleportMurderer: false, causeOfDeath: "SuicideBodyguard");
            target.RpcRemoveModifier<BodyguardGuardModifier>();
        }

        return true;
    }

    private static void ResetButtonTimer(PlayerControl source, CustomActionButton<PlayerControl>? button = null)
    {
        if (!source.AmOwner)
        {
            return;
        }

        var reset = OptionGroupSingleton<GeneralOptions>.Instance.TempSaveCdReset;

        button?.SetTimer(reset);
        source.SetKillTimer(reset);
    }
}