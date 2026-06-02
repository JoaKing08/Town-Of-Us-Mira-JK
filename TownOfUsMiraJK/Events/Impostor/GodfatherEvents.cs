using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Networking;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUsMiraJK.Events.Impostor;

public static class GodfatherEvents
{
    [RegisterEvent(400)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (@event.Player == null)
        {
            return;
        }

        if (@event.Player.Data.Role is not GodfatherRole godfatherRole
            || !OptionGroupSingleton<GodfatherOptions>.Instance.MafiosoDies || godfatherRole.Mafioso == null
            || godfatherRole.Mafioso.Player.HasDied())
        {
            return;
        }

        switch (@event.DeathReason)
        {
            case DeathReason.Exile:
                DeathHandlerModifier.UpdateDeathHandlerImmediate(godfatherRole.Mafioso.Player, TouLocale.Get("DiedToFamily"),
                    DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse,
                    lockInfo: DeathHandlerOverride.SetTrue);
                godfatherRole.Mafioso.Player.Exiled();
                break;
            case DeathReason.Kill:
                var showAnim = MeetingHud.Instance == null && ExileController.Instance == null;
                var murderResultFlags2 = MurderResultFlags.DecisionByHost | MurderResultFlags.Succeeded;

                DeathHandlerModifier.UpdateDeathHandlerImmediate(godfatherRole.Mafioso.Player, TouLocale.Get("DiedToFamily"),
                    DeathEventHandlers.CurrentRound,
                    !MeetingHud.Instance && !ExileController.Instance
                        ? DeathHandlerOverride.SetTrue
                        : DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
                godfatherRole.Mafioso.Player.CustomMurder(
                    godfatherRole.Mafioso.Player,
                    murderResultFlags2,
                    false,
                    showAnim,
                    false,
                    showAnim,
                    false);
                break;
        }
    }
    [RegisterEvent(2)]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;

        if (!source.IsRole<MafiosoRole>())
        {
            return;
        }

        source.SetKillTimer(source.GetKillCooldown() + OptionGroupSingleton<GodfatherOptions>.Instance.KillCooldownDebuff);
    }

    [RegisterEvent(1)]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!PlayerControl.LocalPlayer.IsRole<MafiosoRole>())
        {
            return;
        }

        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown() + OptionGroupSingleton<GodfatherOptions>.Instance.KillCooldownDebuff);
    }
}