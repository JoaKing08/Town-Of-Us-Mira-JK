using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Networking;
using TownOfUs.Options;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Events.Crewmate;

public static class CrusaderEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var target = @event.Target;

        if (target.Data.Role is CrusaderRole)
        {
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x.HasModifier<CrusaderFortifyModifier>(y => y.Crusader == target)))
            {
                player.RemoveModifier<CrusaderFortifyModifier>(x => x.Crusader == target);
            }
        }
    }
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick())
        {
            return;
        }

        CheckForCrusaderFortify(@event, target, PlayerControl.LocalPlayer);
    }

    [RegisterEvent]
    public static void MiraButtonCancelledEventHandler(MiraButtonCancelledEvent @event)
    {
        var source = PlayerControl.LocalPlayer;
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;

        if (target && !target!.HasModifier<CrusaderFortifyModifier>() && button != null)
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

        if (CheckForCrusaderFortify(@event, target, source))
        {
            ResetButtonTimer(source);
        }
    }

    private static bool CheckForCrusaderFortify(MiraCancelableEvent @event, PlayerControl target,
        PlayerControl source)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return false;
        }

        if (!target.HasModifier<CrusaderFortifyModifier>() ||
            target.PlayerId == source.PlayerId ||
            (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield))
        {
            return false;
        }

        MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Error, $"{target.Data.PlayerName} has a crusader fortify, stopping an attack from {source.Data.PlayerName}!");
        @event.Cancel();

        var bodyguard = target.GetModifier<CrusaderFortifyModifier>()?.Crusader.GetRole<CrusaderRole>();

        if (bodyguard != null && (TutorialManager.InstanceExists || source.AmOwner))
        {
            bodyguard.Player.RpcSpecialMurder(source, true, teleportMurderer: false, causeOfDeath: "Crusader");
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