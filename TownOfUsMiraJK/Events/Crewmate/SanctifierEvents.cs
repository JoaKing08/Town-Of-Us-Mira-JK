using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Options;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Buttons.Modifier;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Events.Crewmate;

public static class SanctifierEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.AmOwner && @event.Player.Data.Role is SanctifierRole &&
            OptionGroupSingleton<SanctifierOptions>.Instance.TaskUses &&
            !OptionGroupSingleton<SanctifierOptions>.Instance.ResetSanctify)
        {
            var button = CustomButtonSingleton<SanctifierSanctifyButton>.Instance;
            ++button.UsesLeft;
            ++button.ExtraUses;
            button.SetUses(button.UsesLeft);
        }
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (OptionGroupSingleton<SanctifierOptions>.Instance.ResetSanctify)
        {
            SanctifierCircleManager.Clear();

            if (PlayerControl.LocalPlayer.Data.Role is SanctifierRole)
            {
                var uses = OptionGroupSingleton<SanctifierOptions>.Instance.MaxSanctifies;
                CustomButtonSingleton<SanctifierSanctifyButton>.Instance.SetUses((int)uses);
            }
        }
    }
    [RegisterEvent(-800)]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button;
        var source = PlayerControl.LocalPlayer;
        var target = (button as CustomActionButton<PlayerControl>)?.Target;

        if (button == null || !button.CanClick())
        {
            return;
        }

        CheckForSanctifierCircle(@event, source, target);
    }

    [RegisterEvent(-800)]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;
        if (source == null || target == source)
        {
            return;
        }
        CheckForSanctifierCircle(@event, source, target);
    }
    private static void CheckForSanctifierCircle(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (source.HasDied())
        {
            return;
        }

        if (!source.HasModifier<SanctifiedModifier>() && (target?.HasModifier<SanctifiedModifier>() != true ||
            (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield)))
        {
            return;
        }

        if (@event is MiraButtonClickEvent buttonClickEvent && buttonClickEvent.Button != null && (buttonClickEvent.Button is EngineerVentButton || buttonClickEvent.Button is ExplorerVentButton || buttonClickEvent.Button is FakeVentButton))
        {
            return;
        }

        @event.Cancel();

        if (@event is MiraButtonClickEvent buttonClick)
        {
            var button = buttonClick.Button;
            if (button != null)
            {
                button.Timer = OptionGroupSingleton<GameMechanicOptions>.Instance.TempSaveCdReset;
            }
        }

        if (@event is BeforeMurderEvent && source.IsImpostor())
        {
            source.SetKillTimer(OptionGroupSingleton<GameMechanicOptions>.Instance.TempSaveCdReset);
        }


        if (TutorialManager.InstanceExists || source.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(OptionGroupSingleton<GameMechanicOptions>.Instance.AnonymousShields ? TownOfUsColors.NeutralWiki : TownOfUsMiraJKColors.Sanctifier));
        }
    }
}