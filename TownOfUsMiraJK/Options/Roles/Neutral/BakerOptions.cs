using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUs.Modules.Localization;

namespace TownOfUsMiraJK.Options.Roles.Neutral;

public sealed class BakerOptions : AbstractOptionGroup<BakerRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleBaker", "Baker");

    [ModdedNumberOption("TouJKBakerBreadToFamine", 3, 7, 1)]
    public float BreadToFamine { get; set; } = 5;

    [ModdedNumberOption("TouJKBakerBreadCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BreadCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKBakerBreadLasts", 1, 5, 1)]
    public float BreadLasts { get; set; } = 3;

    [ModdedEnumOption("TouJKBakerNotEnoughPlayers", typeof(NotEnoughPlayersEffect), ["TouJKBakerNotEnoughPlayersEnumTransforms", "TouJKBakerNotEnoughPlayersEnumNothing", "TouJKBakerNotEnoughPlayersEnumDies"])]
    public NotEnoughPlayersEffect NotEnoughPlayersEffect { get; set; } = NotEnoughPlayersEffect.Transforms;

    [ModdedNumberOption("TouJKBakerAnnounceTransformationDelay", 0f, 30f, 1f, MiraNumberSuffixes.Seconds)]
    public float AnnounceTransformationDelay { get; set; } = 10f;

    [ModdedToggleOption("TouJKBakerFamineCanVent")]
    public bool CanVent { get; set; } = false;

    [ModdedNumberOption("TouJKBakerFamineStarveCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float StarveCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKBakerStarveStrength", 1, 5, 1)]
    public float StarveStrength { get; set; } = 1;

    [ModdedNumberOption("TouJKBakerPassiveStarvingCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PassiveStarvingCooldown { get; set; } = 35f;
}
public enum NotEnoughPlayersEffect
{
    Nothing,
    Transforms,
    Dies
}