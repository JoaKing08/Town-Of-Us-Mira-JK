using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using TownOfUs.Buttons.Impostor;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Roles.Neutral;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Events.Neutral;

public static class BerserkerEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        if (!source.AmOwner || source.Data.Role is not BerserkerRole berserker || MeetingHud.Instance)
        {
            return;
        }

        berserker.KillCount++;
        CustomButtonSingleton<BerserkerKillButton>.Instance.ResetCooldownAndOrEffect();
        if (berserker.KillCount >= OptionGroupSingleton<BerserkerOptions>.Instance.KillsToTransform)
        {
            WarRole.RpcTriggerWar(berserker.Player);
            CustomButtonSingleton<WarKillButton>.Instance.SetTimer(OptionGroupSingleton<BerserkerOptions>
                .Instance.WarKillCooldown);
        }
    }
}