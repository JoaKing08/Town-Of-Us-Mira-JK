using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using TownOfUs.Buttons.Impostor;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Modifiers.Game.Crewmate;
using TownOfUs.Options.Modifiers.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Modifiers.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Events.Neutral;

public static class BloodhoundEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        if (!source.AmOwner || source.Data.Role is not BloodhoundRole bloodhound || MeetingHud.Instance)
        {
            return;
        }

        bloodhound.KillCount++;
        if (bloodhound.KillCount >= OptionGroupSingleton<BloodhoundOptions>.Instance.KillsToBloodlust)
        {
            BloodhoundRole.RpcTriggerBloodlust(bloodhound.Player);
            if (@event.Target.HasModifier<DiseasedModifier>())
            {
                CustomButtonSingleton<BloodhoundKillButton>.Instance.SetTimer(OptionGroupSingleton<BloodhoundOptions>.Instance.BloodlustCooldown * OptionGroupSingleton<DiseasedOptions>.Instance.CooldownMultiplier);
            }
        }
    }
}