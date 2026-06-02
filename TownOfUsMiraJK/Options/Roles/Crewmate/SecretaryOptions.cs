using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class SecretaryOptions : AbstractOptionGroup<SecretaryRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleSecretary", "Secretary");

    [ModdedNumberOption("TouJKOptionSecretaryInitialVotes", 0, 10, 1)]
    public float InitialVotes { get; set; } = 2;

    [ModdedNumberOption("TouJKOptionSecretaryMaxVotes", 0, 15, 1, zeroInfinity: true)]
    public float MaxVotes { get; set; } = 5;
}