using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUsMiraJK.Options.Roles.Impostor;

public sealed class PsychopathOptions : AbstractOptionGroup<PsychopathRole>
{
    public override string GroupName => TouLocale.Get("TouJKRolePsychopath", "Psychopath");

    [ModdedNumberOption("TouJKOptionPsychopathInsanityCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float InsanityCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionPsychopathInsanityDuration", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float InsanityDuration { get; set; } = 20f;

    [ModdedNumberOption("TouJKOptionPsychopathInsanityKillCooldown", 2.5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float InsanityKillCooldown { get; set; } = 10f;
}