using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Impostor;

namespace TownOfUsMiraJK.Options.Roles.Impostor;

public sealed class DemagogueOptions : AbstractOptionGroup<DemagogueRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleDemagogue", "Demagogue");

    [ModdedNumberOption("TouJKOptionDemagogueKillCooldownDebuff", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldownDebuff { get; set; } = 10f;

    [ModdedToggleOption("TouJKOptionDemagoguePunishVoters")]
    public bool PunishVoters { get; set; } = true;

    [ModdedToggleOption("TouJKOptionDemagogueCanBeKilledCrew")]
    public bool CanBeKilledCrew { get; set; } = false;

    [ModdedToggleOption("TouJKOptionDemagogueCanBeKilledNonCrew")]
    public bool CanBeKilledNonCrew { get; set; } = true;

    [ModdedToggleOption("TouJKOptionDemagogueGiveHints")]
    public bool GiveHints { get; set; } = true;

    [ModdedToggleOption("TouJKOptionDemagogueAnnounceImmunityDeath")]
    public bool AnnounceImmunityDeath { get; set; } = false;
}