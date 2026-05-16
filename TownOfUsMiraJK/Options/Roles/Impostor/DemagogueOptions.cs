using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Impostor;

public sealed class DemagogueOptions : AbstractOptionGroup<DemagogueRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleDemagogue", "Demagogue");

    [ModdedNumberOption("TouJKOptionDemagogueKillCooldownDebuff", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldownDebuff { get; set; } = 10f;

    [ModdedToggleOption("TouJKOptionDemagoguePunishVoters")]
    public bool PunishVoters { get; set; } = true;

    [ModdedToggleOption("TouJKOptionDemagogueCanBeKilled")]
    public bool CanBeKilled { get; set; } = false;
    public ModdedToggleOption PunishKillers { get; set; } =
        new("TouJKOptionDemagoguePunishKillers", true)
        {
            Visible = () => OptionGroupSingleton<DemagogueOptions>.Instance.CanBeKilled
        };
    public ModdedToggleOption PunishNonCrew { get; set; } =
        new("TouJKOptionDemagoguePunishNonCrew", true)
        {
            Visible = () => OptionGroupSingleton<DemagogueOptions>.Instance.CanBeKilled && OptionGroupSingleton<DemagogueOptions>.Instance.PunishKillers
        };

    [ModdedToggleOption("TouJKOptionDemagogueGiveHints")]
    public bool GiveHints { get; set; } = true;

    [ModdedToggleOption("TouJKOptionDemagogueAnnounceImmunityDeath")]
    public bool AnnounceImmunityDeath { get; set; } = false;
}