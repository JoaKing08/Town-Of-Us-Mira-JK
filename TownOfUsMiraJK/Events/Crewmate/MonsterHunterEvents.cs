using AmongUs.GameOptions;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Buttons.Crewmate;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;

namespace TownOfUs.Events.Crewmate;

public static class MonsterHunterEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;

        if (source.Data.Role is MonsterHunterRole)
        {
            if (GameHistory.PlayerStats.TryGetValue(source.PlayerId, out var stats))
            {
                stats.CorrectKills += 1;
            }
        }
    }

    [RegisterEvent]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        foreach (var mh in CustomRoleUtils.GetActiveRolesOfType<MonsterHunterRole>())
        {
            if (CustomRoleUtils.GetActiveRoles().Any(x => !x.Player.Data.IsDead && !x.Player.Data.Disconnected && (x is VampireRole || x is WerewolfRole || x is NecromancerRole)))
            {
                ushort roleId = OptionGroupSingleton<MonsterHunterOptions>.Instance.BecomesOnMonsterDeath switch
                {
                    BecomesOnMonsterDeath.Deputy => RoleId.Get<DeputyRole>(),
                    BecomesOnMonsterDeath.Hunter => RoleId.Get<HunterRole>(),
                    BecomesOnMonsterDeath.Officer => RoleId.Get<OfficerRole>(),
                    BecomesOnMonsterDeath.Sheriff => RoleId.Get<SheriffRole>(),
                    BecomesOnMonsterDeath.Veteran => RoleId.Get<VeteranRole>(),
                    BecomesOnMonsterDeath.Vigilante => RoleId.Get<VigilanteRole>(),
                    BecomesOnMonsterDeath.Gunslinger => RoleId.Get<DeputyRole>(),
                    _ => (ushort)RoleTypes.Crewmate
                };
                mh.Player.ChangeRole(roleId);
            }
        }
    }
}