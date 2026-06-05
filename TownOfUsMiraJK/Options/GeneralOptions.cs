using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace TownOfUs.Options;

public sealed class GeneralJKOptions : AbstractOptionGroup
{
    public override string GroupName => "General";
    public override uint GroupPriority => 1;

    public ModdedToggleOption ApocTeam { get; set; } = new("TouJKOptionApocTeam", true);

    public ModdedToggleOption ApocChat { get; set; } = new("TouJKOptionApocChat", true)
    {
        Visible = () => OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam
    };
}