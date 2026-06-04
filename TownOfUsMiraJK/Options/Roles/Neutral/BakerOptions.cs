using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class BakerOptions : AbstractOptionGroup<BakerRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleBaker", "Baker");

    [ModdedNumberOption("TouJKOptionBakerBreadToFamine", 3, 7, 1)]
    public float BreadToFamine { get; set; } = 5;

    [ModdedNumberOption("TouJKOptionBakerBreadCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BreadCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionBakerBreadLasts", 1, 5, 1)]
    public float BreadLasts { get; set; } = 3;

    [ModdedEnumOption("TouJKOptionBakerNotEnoughPlayers", typeof(NotEnoughPlayersEffect), ["TouJKOptionBakerNotEnoughPlayersEnumTransforms", "TouJKOptionBakerNotEnoughPlayersEnumNothing", "TouJKOptionBakerNotEnoughPlayersEnumDies"])]
    public NotEnoughPlayersEffect NotEnoughPlayersEffect { get; set; } = NotEnoughPlayersEffect.Transforms;

    [ModdedNumberOption("TouJKOptionBakerAnnounceTransformationDelay", 0f, 30f, 1f, MiraNumberSuffixes.Seconds)]
    public float AnnounceTransformationDelay { get; set; } = 10f;

    [ModdedToggleOption("TouJKOptionBakerFamineCanVent")]
    public bool CanVent { get; set; } = false;

    [ModdedNumberOption("TouJKOptionBakerFamineStarveCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float StarveCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKOptionBakerStarveStrength", 1, 5, 1)]
    public float StarveStrength { get; set; } = 1;

    [ModdedNumberOption("TouJKOptionBakerPassiveStarvingCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PassiveStarvingCooldown { get; set; } = 35f;
}
public enum NotEnoughPlayersEffect
{
    Nothing,
    Transforms,
    Dies
}