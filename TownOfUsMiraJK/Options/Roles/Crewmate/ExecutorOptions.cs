using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUs.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class ExecutorOptions : AbstractOptionGroup<ExecutorRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleExecutor", "Executor");

    [ModdedNumberOption("TouJKOptionExecutorAimCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float AimCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionExecutorAimMaxUses", 1, 15, 1)]
    public float AimMaxUses { get; set; } = 5;

    [ModdedToggleOption("TouJKOptionExecutorReveal")]
    public bool Reveal { get; set; } = false;
}