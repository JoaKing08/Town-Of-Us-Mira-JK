using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Buttons;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Modifiers.Neutral;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUsMiraJK.Events.Impostor;

public static class PsychopathEvents
{
    [RegisterEvent(5)]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (@event.Source.HasModifier<PsychopathInsanityModifier>())
        {
            @event.Source.SetKillTimer(OptionGroupSingleton<PsychopathOptions>.Instance.InsanityKillCooldown);
        }
    }

    [RegisterEvent(5)]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (PlayerControl.LocalPlayer.HasModifier<PsychopathInsanityModifier>())
        {
            PlayerControl.LocalPlayer.SetKillTimer(OptionGroupSingleton<PsychopathOptions>.Instance.InsanityKillCooldown);
        }
    }
}