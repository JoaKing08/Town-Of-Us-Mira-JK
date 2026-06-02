using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Buttons.Impostor;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUsMiraJK.Events.Impostor;

public static class SniperEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;
        if (source.IsImpostor() && source.AmOwner && source != target && !MeetingHud.Instance && source.IsRole<SniperRole>())
        {
            var shootButton = CustomButtonSingleton<SniperShootButton>.Instance;
            shootButton.ResetCooldownAndOrEffect();
        }
    }
}