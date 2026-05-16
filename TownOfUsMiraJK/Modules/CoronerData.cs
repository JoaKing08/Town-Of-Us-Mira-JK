using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Patches;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using UnityEngine;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;
using Object = UnityEngine.Object;

namespace TownOfUs.Modules;

[HarmonyPatch]
public class CoronerData(byte victimId, byte killerId, bool indirect = false, bool vented = false, RoleTypes killerRole = RoleTypes.Impostor, RoleTypes victimRole = RoleTypes.Crewmate, string killerColor = "unknown", int otherKills = 0)
{
    public CoronerData Copy()
    {
        var copy = new CoronerData(VictimId, KillerId, IsIndirect, Vented, KillerRole, VictimRole, KillerColor, KillerOtherKills)
        {
            KillTime = KillTime,
            KillerEscaped = KillerEscaped,
            AbilityEscape = AbilityEscape,
            lastRecordedKillerPosition = lastRecordedKillerPosition,
            VictimName = VictimName
        };
        return copy;
    }
    public byte VictimId { get; set; } = victimId;
    public byte KillerId { get; set; } = killerId;
    public DateTime KillTime { get; set; } = DateTime.UtcNow;
    public float KilledAgo => (float?)(DateTime.UtcNow - KillTime).TotalMilliseconds ?? 0f;
    public KillerEscape KillerEscaped { get; set; } = KillerEscape.Nowhere;
    public bool IsIndirect { get; set; } = indirect;
    public bool AbilityEscape { get; set; }
    public bool Vented { get; set; } = vented;
    public RoleTypes KillerRole { get; set; } = killerRole;
    public RoleTypes VictimRole { get; set; } = victimRole;
    public string KillerColor { get; set; } = killerColor;
    public Vector3? lastRecordedKillerPosition { get; set; }
    public int KillerOtherKills { get; set; } = otherKills;
    public string VictimName { get; set; } = MiscUtils.PlayerById(victimId)?.Data?.PlayerName ?? "";
    public KillerKills KillerStatus
    {
        get
        {
            var stats = GameHistory.PlayerStats[KillerId];
            if (KillerOtherKills >= 5)
            {
                return KillerKills.Mass;
            }
            else if (KillerOtherKills >= 3)
            {
                return KillerKills.Many;
            }
            else if (KillerOtherKills >= 1)
            {
                return KillerKills.Few;
            }
            else
            {
                return KillerKills.None;
            }
        }
    }

    public static readonly List<CoronerData> AllData = [];

    [HarmonyPatch(typeof(GameHistory), nameof(GameHistory.AddMurder))]
    [HarmonyPostfix]
    public static void AddMurder(PlayerControl killer, PlayerControl victim)
    {
        if (!CustomRoleUtils.GetActiveRolesOfType<CoronerRole>().Any())
        {
            return;
        }
        var coroData = new CoronerData(victim.PlayerId, killer.PlayerId, killer.HasModifier<IndirectAttackerModifier>(), killer.inVent, killer.Data.Role.Role, victim.GetRoleWhenAlive().Role, MedicRole.GetColorTypeForPlayer(killer), GameHistory.KilledPlayers.Count(x => x.KillerId == killer.PlayerId && x.VictimId != killer.PlayerId && x.VictimId != victim.PlayerId));

        AllData.Add(coroData);
    }

    [HarmonyPatch(typeof(GameHistory), nameof(GameHistory.ClearMurder))]
    [HarmonyPrefix]
    public static void ClearMurder(PlayerControl player)
    {
        var instance = AllData
            .Where(x => x.VictimId == player.PlayerId)
            .OrderByDescending(x => x.KillTime)
            .FirstOrDefault();

        if (instance == null)
        {
            return;
        }

        AllData.Remove(instance);
        var ownerId = player.GetModifier<DemagogueImmunityModifier>()?.OwnerId;
        if (ownerId != null)
        {
            var dem = MiscUtils.PlayerById((byte)ownerId)?.GetRole<DemagogueRole>();
            if (dem != null && !OptionGroupSingleton<DemagogueOptions>.Instance.CanBeKilled && !dem.Player.HasModifier<InvulnerabilityModifier>())
            {
                dem.invulnerabilityModifier = player.AddModifier<InvulnerabilityModifier>(false, false, false)!;
            }
        }
    }

    [HarmonyPatch(typeof(GameHistory), nameof(GameHistory.ClearAll))]
    [HarmonyPrefix]
    public static void ClearAll()
    {
        AllData.Clear();
    }

    public void Update()
    {
        var body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == VictimId);
        if (body == null)
        {
            return;
        }

        var killer = MiscUtils.PlayerById(KillerId);
        if (killer == null)
        {
            return;
        }

        if (KilledAgo <= OptionGroupSingleton<CoronerOptions>.Instance.KillerUpdate * 1000f)
        {
            if (killer.inVent)
            {
                Vented = true;
            }
            else if (lastRecordedKillerPosition != null && Vector3.Distance(killer.transform.position, (Vector3)lastRecordedKillerPosition) > killer.GetAppearance().Speed)
            {
                AbilityEscape = true;
            }
        }
        lastRecordedKillerPosition = killer.transform.position;

        if ((KilledAgo <= OptionGroupSingleton<CoronerOptions>.Instance.KillerUpdate * 1000f || KillerEscaped == KillerEscape.Nowhere) && !IsIndirect && !AbilityEscape && !Vented)
        {
            var distanceX = killer.transform.position.x - body.transform.position.x;
            var distanceY = killer.transform.position.y - body.transform.position.y;
            var absoluteDistanceX = MathF.Abs(distanceX);
            var absoluteDistanceY = MathF.Abs(distanceY);
            if (Vector3.Distance(killer.transform.position, body.transform.position) >= GameManager.Instance.LogicOptions.GetKillDistance() * 3)
            {
                if (absoluteDistanceX > absoluteDistanceY)
                {
                    if (distanceX < 0)
                    {
                        KillerEscaped = KillerEscape.West;
                    }
                    else
                    {
                        KillerEscaped = KillerEscape.East;
                    }
                }
                else
                {
                    if (distanceY < 0)
                    {
                        KillerEscaped = KillerEscape.South;
                    }
                    else
                    {
                        KillerEscaped = KillerEscape.North;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    [HarmonyPostfix]
    public static void UpdatePatch(PlayerControl __instance)
    {
        if (!__instance.AmOwner)
        {
            return;
        }
        foreach (var data in AllData)
        {
            data.Update();
        }
    }
}