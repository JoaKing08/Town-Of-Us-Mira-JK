using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Networking;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Neutral;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.GameOptions;
using TownOfUsMiraJK.Modules.Components;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Neutral.WitchReveal;

public sealed class CrewmateFaction(IntPtr cppPtr)
    : CrewmateRole(cppPtr), ICustomRole
{
    public string RoleName => TouLocale.Get("CrewmateKeyword");
    public Color RoleColor => Palette.CrewmateBlue;
    public string RoleDescription => "";
    public string RoleLongDescription => "";
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Crewmate,
        ShowInFreeplay = false,
        HideSettings = true
    };
}
public sealed class NeutralFaction(IntPtr cppPtr)
    : NeutralRole(cppPtr), ICustomRole
{
    public string RoleName => TouLocale.Get("NeutralKeyword");
    public Color RoleColor => TownOfUsColors.Neutral;
    public string RoleDescription => "";
    public string RoleLongDescription => "";
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Neutral,
        ShowInFreeplay = false,
        HideSettings = true
    };
}
public sealed class ImpostorFaction(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ICustomRole
{
    public string RoleName => TouLocale.Get("ImpostorKeyword");
    public Color RoleColor => Palette.ImpostorRed;
    public string RoleDescription => "";
    public string RoleLongDescription => "";
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Impostor,
        ShowInFreeplay = false,
        HideSettings = true
    };
}