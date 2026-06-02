using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class WatcherOptions : AbstractOptionGroup<WatcherRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleWatcher", "Watcher");

    [ModdedNumberOption("TouJKWatcherWatchCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float WatchCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKWatcherWatchDuration", 2.5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float WatchDuration { get; set; } = 10f;

    [ModdedNumberOption("TouJKWatcherWatchVisionMult", 1f, 3f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float WatchVisionMult { get; set; } = 1.25f;
}