using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUsMiraJK.Options.Roles.Impostor;

public sealed class GodfatherOptions : AbstractOptionGroup<GodfatherRole>
{
    public override string GroupName => TouLocale.Get("TouJKRolePoisoner", "Poisoner");

    [ModdedNumberOption("TouJKOptionGodfatherMafiosoKillCooldownDebuff", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldownDebuff { get; set; } = 10f;

    [ModdedToggleOption("TouJKOptionGodfatherCanKill")]
    public bool CanKill { get; set; } = false;

    public ModdedToggleOption CanKillBeforeMafioso { get; set; } =
        new("TouJKOptionGodfatherCanKillBeforeMafioso", false)
        {
            Visible = () => !OptionGroupSingleton<GodfatherOptions>.Instance.CanKill
        };

    [ModdedToggleOption("TouJKOptionGodfatherMafiosoDiesWithGodfather")]
    public bool MafiosoDies { get; set; } = true;
}