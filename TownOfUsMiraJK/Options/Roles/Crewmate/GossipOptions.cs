using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUsMiraJK.Options.Roles.Crewmate;

public sealed class GossipOptions : AbstractOptionGroup<GossipRole>
{
    public override string GroupName => TouLocale.Get("TouJKRoleGossip", "Gossip");

    [ModdedNumberOption("TouJKGossipChatCooldown", 5f, 120f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ChatCooldown { get; set; } = 25f;

    [ModdedNumberOption("TouJKGossipPlayers", 2f, 5f, 1f)]
    public float Players { get; set; } = 3f;

    [ModdedToggleOption("TouJKGossipUseDead")]
    public bool UseDead { get; set; } = false;
}