using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class MonsterHunterOptions : AbstractOptionGroup<MonsterHunterRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleMonsterHunter", "Monster Hunter");

    [ModdedNumberOption("TouJKMonsterHunterStakeCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float StakeCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKMonsterHunterMaxStakes", 1, 10, 1)]
    public float MaxStakes { get; set; } = 5;

    [ModdedToggleOption("TouJKMonsterHunterStakeRoundOne")]
    public bool StakeRoundOne { get; set; } = false;

    [ModdedToggleOption("TouJKMonsterHunterSuicide")]
    public bool Suicide { get; set; } = false;

    [ModdedEnumOption("TouJKMonsterHunterOnMonsterDeath", typeof(BecomesOnMonsterDeath), ["CrewmateKeyword", "TouRoleDeputy", "TouRoleHunter", "TouRoleOfficer", "TouRoleSheriff", "TouRoleVeteran", "TouRoleVigilante", "TouJKRoleExecutor"])]
    public BecomesOnMonsterDeath BecomesOnMonsterDeath { get; set; } = BecomesOnMonsterDeath.Crewmate;
}
public enum BecomesOnMonsterDeath
{
    Crewmate,
    Deputy,
    Hunter,
    Officer,
    Sheriff,
    Veteran,
    Vigilante,
    Executor
}