using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class InspectorOptions : AbstractOptionGroup<InspectorRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleInspector", "Inspector");

    [ModdedNumberOption("TouJKOptionInspectorInspectCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float InspectorCooldown { get; set; } = 25f;

    [ModdedToggleOption("TouJKOptionInspectorUseDoomResults")]
    public bool UseDoomResults { get; set; } = true;

    public ModdedNumberOption CrewRoles { get; set; } =
        new("TouJKOptionInspectorCrewRoles", 3, 1, 8, 1, MiraNumberSuffixes.None)
        {
            Visible = () => !OptionGroupSingleton<InspectorOptions>.Instance.UseDoomResults
        };

    public ModdedNumberOption NeutRoles { get; set; } =
        new("TouJKOptionInspectorNeutRoles", 2, 1, 8, 1, MiraNumberSuffixes.None)
        {
            Visible = () => !OptionGroupSingleton<InspectorOptions>.Instance.UseDoomResults
        };

    public ModdedNumberOption ImpRoles { get; set; } =
        new("TouJKOptionInspectorImpRoles", 2, 1, 8, 1, MiraNumberSuffixes.None)
        {
            Visible = () => !OptionGroupSingleton<InspectorOptions>.Instance.UseDoomResults
        };
}