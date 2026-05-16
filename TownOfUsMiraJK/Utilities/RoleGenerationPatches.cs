using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Events;
using TownOfUs.Extensions;
using TownOfUs.Options;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Utilities
{
    [HarmonyPatch]
    public static class RoleGenerationPatches
    {
        [HarmonyPatch(typeof(TouRoleManagerPatches), "AssignRoles")]
        [HarmonyPrefix]
        private static bool AssignRoles(List<NetworkedPlayerInfo> infected)
        {
            var impCount = infected.Count;
            var impostors = MiscUtils.GetImpostors(infected);
            var crewmates = MiscUtils.GetCrewmates(impostors);

            var roleOptions = OptionGroupSingleton<RoleOptions>.Instance;
            var roleOptions2 = OptionGroupSingleton<TownOfUsMiraJK.Options.RoleJKOptions>.Instance;
            var nbCount = UnityEngine.Random.RandomRange((int)roleOptions.MinNeutralBenign.Value,
                (int)roleOptions.MaxNeutralBenign.Value + 1);
            var neCount = UnityEngine.Random.RandomRange((int)roleOptions.MinNeutralEvil.Value,
                (int)roleOptions.MaxNeutralEvil.Value + 1);
            var nkCount = UnityEngine.Random.RandomRange((int)roleOptions.MinNeutralKiller.Value,
                (int)roleOptions.MaxNeutralKiller.Value + 1);
            var noCount = UnityEngine.Random.RandomRange((int)roleOptions.MinNeutralOutlier.Value,
                (int)roleOptions.MaxNeutralOutlier.Value + 1);
            var naCount = UnityEngine.Random.RandomRange((int)roleOptions2.MinNeutralApocalypse.Value,
                (int)roleOptions2.MaxNeutralApocalypse.Value + 1);

            AdjustNeutralCountsForUpRequests(ref nbCount, ref neCount, ref nkCount, ref noCount, ref naCount, crewmates.Count);

            var excluded = MiscUtils.SpawnableRoles.Where(x => x is ISpawnChange { NoSpawn: true }).Select(x => x.Role);

            var impRoles =
                MiscUtils.GetMaxRolesToAssign(ModdedRoleTeams.Impostor, impCount, x => !excluded.Contains(x.Role));

            var uniqueRole = MiscUtils.SpawnableRoles.FirstOrDefault(x => x is ISpawnChange { NoSpawn: false });
            if (uniqueRole != null && impRoles.Contains(RoleId.Get(uniqueRole.GetType())))
            {
                impCount = 1;
                var impText = $"Removing Impostor Roles because of {uniqueRole.GetRoleName()}";
                MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Warning, impText);

                impRoles.RemoveAll(x => x != RoleId.Get(uniqueRole.GetType()));

                while (impostors.Count > impCount)
                {
                    crewmates.Add(impostors.TakeFirst());
                }
            }

            var nbRoles = MiscUtils.GetMaxRolesToAssign(RoleAlignment.NeutralBenign, nbCount);
            var neRoles = MiscUtils.GetMaxRolesToAssign(RoleAlignment.NeutralEvil, neCount);
            var nkRoles = MiscUtils.GetMaxRolesToAssign(RoleAlignment.NeutralKilling, nkCount);
            var noRoles = MiscUtils.GetMaxRolesToAssign(RoleAlignment.NeutralOutlier, noCount);
            var naRoles = MiscUtils.GetMaxRolesToAssign((RoleAlignment)27, naCount);

            var crewCount = crewmates.Count - nbRoles.Count - neRoles.Count - nkRoles.Count - noRoles.Count - naRoles.Count;
            var crewRoles = MiscUtils.GetMaxRolesToAssign(ModdedRoleTeams.Crewmate, crewCount);
            if (!nkRoles.Any(x => x == RoleId.Get<VampireRole>() || x == RoleId.Get<WerewolfRole>()) && !noRoles.Any(x => x == RoleId.Get<NecromancerRole>()))
            {
                if (crewRoles.Contains(RoleId.Get<MonsterHunterRole>()))
                {
                    crewRoles = MiscUtils.GetMaxRolesToAssign(ModdedRoleTeams.Crewmate, crewCount, (x) => (ushort)x.Role != RoleId.Get<MonsterHunterRole>());
                }
            }

            var crewAndNeutRoles = new List<ushort>();
            crewAndNeutRoles.AddRange(nbRoles);
            crewAndNeutRoles.AddRange(neRoles);
            crewAndNeutRoles.AddRange(nkRoles);
            crewAndNeutRoles.AddRange(noRoles);
            crewAndNeutRoles.AddRange(naRoles);
            crewAndNeutRoles.AddRange(crewRoles);

            AddUpRequestedRolesToPools(impRoles, crewAndNeutRoles);

            AssignRolesToPlayers(crewmates, crewAndNeutRoles, "Crewmate/Neutral");
            AssignRolesToPlayers(impostors, impRoles, "Impostor");

            AssignVanillaRoles(crewmates, impostors);
            return false;
        }

        // Role lists may spawn normal crewmates if there will be to little roles turned on with monster hunter on.
        [HarmonyPatch(typeof(TouRoleManagerPatches), "AssignRolesFromRoleList")]
        [HarmonyPrefix]
        private static bool AssignRolesFromRoleList(List<NetworkedPlayerInfo> infected)
        {
            var impostors = MiscUtils.GetImpostors(infected);
            var crewmates = MiscUtils.GetCrewmates(impostors);

            var crewRoles = new List<ushort>();
            var impRoles = new List<ushort>();

            var players = impostors.Count + crewmates.Count;

            List<RoleListOption> crewNkBuckets =
            [
                RoleListOption.CrewInvest, RoleListOption.CrewKilling, RoleListOption.CrewPower,
            RoleListOption.CrewProtective, RoleListOption.CrewSupport, RoleListOption.CrewCommon,
            RoleListOption.CrewSpecial, RoleListOption.CrewRandom, RoleListOption.NeutKilling
            ];
            List<RoleListOption> impBuckets =
            [
                RoleListOption.ImpConceal, RoleListOption.ImpKilling, RoleListOption.ImpPower,
            RoleListOption.ImpSupport, RoleListOption.ImpCommon, RoleListOption.ImpSpecial,
            RoleListOption.ImpRandom
            ];

            var buckets = BuildRoleListBuckets(players);
            var wildcardActive = buckets.Any(x => x == RoleListOption.NeutWildcard);
            AdjustRoleListBucketsForImpostors(buckets, impBuckets, impostors.Count);
            EnsureCrewNeutralRoles(buckets, impBuckets, crewNkBuckets);

            var impCount = buckets.Count(bucket => impBuckets.Contains(bucket));


            var excluded = MiscUtils.SpawnableRoles.Where(x => x is ISpawnChange { NoSpawn: true }).Select(x => x.Role).ToList();
            var exclusionFilter = new Func<RoleBehaviour, bool>(x => !excluded.Contains(x.Role));
            var monsterHunterFromBucket = RoleListOption.Any;
            var crewInvestRoles = MiscUtils.GetRolesToAssign(RoleAlignment.CrewmateInvestigative, exclusionFilter);
            var crewKillingRoles = MiscUtils.GetRolesToAssign(RoleAlignment.CrewmateKilling, exclusionFilter);
            var crewProtectRoles = MiscUtils.GetRolesToAssign(RoleAlignment.CrewmateProtective, exclusionFilter);
            var crewPowerRoles = MiscUtils.GetRolesToAssign(RoleAlignment.CrewmatePower, exclusionFilter);
            var crewSupportRoles = MiscUtils.GetRolesToAssign(RoleAlignment.CrewmateSupport, exclusionFilter);
            var neutBenignRoles = MiscUtils.GetRolesToAssign(RoleAlignment.NeutralBenign, exclusionFilter);
            var neutEvilRoles = MiscUtils.GetRolesToAssign(RoleAlignment.NeutralEvil, exclusionFilter);
            var neutKillingRoles = MiscUtils.GetRolesToAssign(RoleAlignment.NeutralKilling, exclusionFilter);
            var neutOutlierRoles = MiscUtils.GetRolesToAssign(RoleAlignment.NeutralOutlier, exclusionFilter);
            var neutApocalypseRoles = MiscUtils.GetRolesToAssign((RoleAlignment)27, exclusionFilter);
            var impConcealRoles = MiscUtils.GetRolesToAssign(RoleAlignment.ImpostorConcealing, exclusionFilter);
            var impKillingRoles = MiscUtils.GetRolesToAssign(RoleAlignment.ImpostorKilling, exclusionFilter);
            var impPowerRoles = MiscUtils.GetRolesToAssign(RoleAlignment.ImpostorPower, exclusionFilter);
            var impSupportRoles = MiscUtils.GetRolesToAssign(RoleAlignment.ImpostorSupport);

            impRoles.AddRange(MiscUtils.ReadFromBucket(buckets, impConcealRoles, RoleListOption.ImpConceal,
                RoleListOption.ImpCommon));

            var commonImpRoles = impConcealRoles;

            impRoles.AddRange(MiscUtils.ReadFromBucket(buckets, impSupportRoles, RoleListOption.ImpSupport,
                RoleListOption.ImpCommon));

            commonImpRoles.AddRange(impSupportRoles);

            impRoles.AddRange(MiscUtils.ReadFromBucket(buckets, impKillingRoles, RoleListOption.ImpKilling,
                RoleListOption.ImpSpecial));

            var specialImpRoles = impKillingRoles;

            impRoles.AddRange(MiscUtils.ReadFromBucket(buckets, impPowerRoles, RoleListOption.ImpPower,
                RoleListOption.ImpSpecial));

            specialImpRoles.AddRange(impPowerRoles);

            impRoles.AddRange(MiscUtils.ReadFromBucket(buckets, commonImpRoles, RoleListOption.ImpCommon,
                RoleListOption.ImpRandom));

            var randomImpRoles = commonImpRoles;

            impRoles.AddRange(MiscUtils.ReadFromBucket(buckets, specialImpRoles, RoleListOption.ImpSpecial,
                RoleListOption.ImpRandom));

            randomImpRoles.AddRange(specialImpRoles);

            impRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomImpRoles, RoleListOption.ImpRandom));

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, crewInvestRoles, RoleListOption.CrewInvest,
                RoleListOption.CrewCommon));

            var commonCrewRoles = crewInvestRoles;

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, crewProtectRoles, RoleListOption.CrewProtective,
                RoleListOption.CrewCommon));

            commonCrewRoles.AddRange(crewProtectRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, crewSupportRoles, RoleListOption.CrewSupport,
                RoleListOption.CrewCommon));

            commonCrewRoles.AddRange(crewSupportRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, crewKillingRoles, RoleListOption.CrewKilling,
                RoleListOption.CrewSpecial));
            if (crewRoles.Contains(RoleId.Get<MonsterHunterRole>()) && monsterHunterFromBucket == RoleListOption.Any)
            {
                monsterHunterFromBucket = RoleListOption.CrewKilling;
            }

            var specialCrewRoles = crewKillingRoles;

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, crewPowerRoles, RoleListOption.CrewPower,
                RoleListOption.CrewSpecial));

            specialCrewRoles.AddRange(crewPowerRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, commonCrewRoles, RoleListOption.CrewCommon,
                RoleListOption.CrewRandom));

            var randomCrewRoles = commonCrewRoles;

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, specialCrewRoles, RoleListOption.CrewSpecial,
                RoleListOption.CrewRandom));
            if (crewRoles.Contains(RoleId.Get<MonsterHunterRole>()) && monsterHunterFromBucket == RoleListOption.Any)
            {
                monsterHunterFromBucket = RoleListOption.CrewSpecial;
            }

            randomCrewRoles.AddRange(specialCrewRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomCrewRoles, RoleListOption.CrewRandom));
            if (crewRoles.Contains(RoleId.Get<MonsterHunterRole>()) && monsterHunterFromBucket == RoleListOption.Any)
            {
                monsterHunterFromBucket = RoleListOption.CrewRandom;
            }

            var randomNonImpRoles = randomCrewRoles;

            List<(ushort RoleType, int Chance)> commonNeutRoles;
            if (wildcardActive)
            {
                crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutBenignRoles, RoleListOption.NeutBenign,
                    RoleListOption.NeutCommon, RoleListOption.NeutWildcard));

                commonNeutRoles = neutBenignRoles;

                crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutEvilRoles, RoleListOption.NeutEvil,
                    RoleListOption.NeutCommon, RoleListOption.NeutWildcard));

                commonNeutRoles.AddRange(neutEvilRoles);

                crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutOutlierRoles, RoleListOption.NeutOutlier,
                    RoleListOption.NeutSpecial, RoleListOption.NeutWildcard));
            }
            else
            {
                crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutBenignRoles, RoleListOption.NeutBenign,
                    RoleListOption.NeutCommon));

                commonNeutRoles = neutBenignRoles;

                crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutEvilRoles, RoleListOption.NeutEvil,
                    RoleListOption.NeutCommon));

                commonNeutRoles.AddRange(neutEvilRoles);

                crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutOutlierRoles, RoleListOption.NeutOutlier,
                    RoleListOption.NeutSpecial));
            }
            var specialNeutRoles = neutOutlierRoles;

            var wildNeutRoles = new List<(ushort RoleType, int Chance)>();

            wildNeutRoles.AddRange(neutOutlierRoles);

            wildNeutRoles.AddRange(commonNeutRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutKillingRoles, RoleListOption.NeutKilling,
                RoleListOption.NeutSpecial));

            specialNeutRoles.AddRange(neutKillingRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutApocalypseRoles, (RoleListOption)25,
                RoleListOption.NeutSpecial));

            specialNeutRoles.AddRange(neutApocalypseRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, commonNeutRoles, RoleListOption.NeutCommon,
                RoleListOption.NeutRandom));

            var randomNeutRoles = commonNeutRoles;

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, specialNeutRoles, RoleListOption.NeutSpecial,
                RoleListOption.NeutRandom));

            randomNeutRoles.AddRange(specialNeutRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, wildNeutRoles, RoleListOption.NeutWildcard,
                RoleListOption.NeutRandom));

            randomNeutRoles.AddRange(wildNeutRoles);

            randomNeutRoles.AddRange(commonNeutRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomNeutRoles, RoleListOption.NeutRandom,
                RoleListOption.NonImp));

            randomNonImpRoles.AddRange(randomNeutRoles);

            crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomNonImpRoles, RoleListOption.NonImp));
            if (crewRoles.Contains(RoleId.Get<MonsterHunterRole>()) && monsterHunterFromBucket == RoleListOption.Any)
            {
                monsterHunterFromBucket = RoleListOption.NonImp;
            }

            if (!crewRoles.Any(x => x == RoleId.Get<WerewolfRole>() || x == RoleId.Get<VampireRole>() || x == RoleId.Get<NecromancerRole>()))
            {
                var rolesToReplace = new List<(ushort RoleType, int Chance)>();
                switch (monsterHunterFromBucket)
                {
                    case RoleListOption.CrewKilling:
                        rolesToReplace = crewKillingRoles.Where(x => !crewRoles.Contains(x.RoleType)).ToList();
                        break;
                    case RoleListOption.CrewSpecial:
                        rolesToReplace = specialCrewRoles.Where(x => !crewRoles.Contains(x.RoleType)).ToList();
                        break;
                    case RoleListOption.CrewRandom:
                        rolesToReplace = randomCrewRoles.Where(x => !crewRoles.Contains(x.RoleType)).ToList();
                        break;
                    case RoleListOption.NonImp:
                        rolesToReplace = randomNonImpRoles.Where(x => !crewRoles.Contains(x.RoleType)).ToList();
                        break;
                    case RoleListOption.Any:
                        rolesToReplace = randomNonImpRoles.Where(x => !crewRoles.Contains(x.RoleType)).ToList();
                        rolesToReplace.AddRange(randomImpRoles.Where(x => !impRoles.Contains(x.RoleType)));
                        break;
                }
                var toReplace = crewRoles.RemoveAll(x => x == RoleId.Get<MonsterHunterRole>());
                if (toReplace > 0)
                {
                    if (crewRoles.Any())
                    {
                        crewRoles.AddRange(MiscUtils.ReadFromBucket(buckets, rolesToReplace, monsterHunterFromBucket).GetRange(0, toReplace));
                    }
                    else
                    {
                        for (int i = 0; i < toReplace; i++)
                        {
                            crewRoles.Add((ushort)RoleTypes.Crewmate);
                        }
                    }
                }
            }

            AddUpRequestedRolesToPools(impRoles, crewRoles);

            crewRoles.Shuffle();
            impRoles.Shuffle();

            var chosenImpRoles = impRoles.Take(impCount).ToList();

            foreach (var impostor in impostors.ToList())
            {
                if (UpCommandRequests.TryGetRequestRole(impostor.Data.PlayerName, out var requestedRole))
                {
                    var requestedRoleId = (ushort)requestedRole.Role;
                    if (requestedRole.IsImpostor() && !chosenImpRoles.Contains(requestedRoleId))
                    {
                        if (chosenImpRoles.Count >= impCount && chosenImpRoles.Count > 0)
                        {
                            var randomIndex = UnityEngine.Random.RandomRangeInt(0, chosenImpRoles.Count);
                            chosenImpRoles.RemoveAt(randomIndex);
                        }
                        chosenImpRoles.Add(requestedRoleId);
                    }
                }
            }

            chosenImpRoles = chosenImpRoles.Pad(impCount, (ushort)RoleTypes.Impostor);

            var uniqueRole = MiscUtils.SpawnableRoles.FirstOrDefault(x => x is ISpawnChange { NoSpawn: false });
            if (uniqueRole != null && chosenImpRoles.Contains(RoleId.Get(uniqueRole.GetType())))
            {
                impCount = 1;
                var impText = $"Removing Impostor Roles because of {uniqueRole.GetRoleName()}";
                MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Warning, impText);

                while (impostors.Count > impCount)
                {
                    crewmates.Add(impostors.TakeFirst());
                }

                chosenImpRoles.RemoveAll(x => x != RoleId.Get(uniqueRole.GetType()));

                foreach (var impostor in impostors.ToList())
                {
                    if (UpCommandRequests.TryGetRequestRole(impostor.Data.PlayerName, out var requestedRole))
                    {
                        var requestedRoleId = (ushort)requestedRole.Role;
                        if (requestedRole.IsImpostor() && !chosenImpRoles.Contains(requestedRoleId))
                        {
                            chosenImpRoles.Add(requestedRoleId);
                        }
                    }
                }
            }

            AssignRolesToPlayers(impostors, chosenImpRoles, "Impostor");
            AssignRolesToPlayers(crewmates, crewRoles, "Crewmate/Neutral");

            AssignVanillaRoles(crewmates, impostors);
            return false;
        }
        private static void AdjustNeutralCountsForUpRequests(ref int nbCount, ref int neCount, ref int nkCount, ref int noCount, ref int naCount, int crewmateCount)
        {
            var upRequestedBenign = false;
            var upRequestedEvil = false;
            var upRequestedKilling = false;
            var upRequestedOutlier = false;
            var upRequestedApocalypse = false;

            var upRequests = UpCommandRequests.GetAllRequests();
            foreach (var (playerName, _) in upRequests)
            {
                if (!UpCommandRequests.TryGetRequestRole(playerName, out var requestedRole))
                {
                    continue;
                }

                if (requestedRole.IsNeutral())
                {
                    var alignment = requestedRole.GetRoleAlignment();

                    switch (alignment)
                    {
                        case RoleAlignment.NeutralBenign:
                            upRequestedBenign = true;
                            if (nbCount == 0)
                            {
                                nbCount = 1;
                            }
                            break;
                        case RoleAlignment.NeutralEvil:
                            upRequestedEvil = true;
                            if (neCount == 0)
                            {
                                neCount = 1;
                            }
                            break;
                        case RoleAlignment.NeutralKilling:
                            upRequestedKilling = true;
                            if (nkCount == 0)
                            {
                                nkCount = 1;
                            }
                            break;
                        case RoleAlignment.NeutralOutlier:
                            upRequestedOutlier = true;
                            if (noCount == 0)
                            {
                                noCount = 1;
                            }
                            break;
                        case (RoleAlignment)27:
                            upRequestedApocalypse = true;
                            if (naCount == 0)
                            {
                                naCount = 1;
                            }
                            break;
                    }
                }
            }

            AdjustNeutralCountsWithProtection(ref nbCount, ref neCount, ref nkCount, ref noCount, ref naCount, crewmateCount,
                upRequestedBenign, upRequestedEvil, upRequestedKilling, upRequestedOutlier, upRequestedApocalypse);
        }

        private static void AdjustNeutralCountsWithProtection(ref int nbCount, ref int neCount, ref int nkCount, ref int noCount, ref int naCount, int crewmateCount,
            bool protectBenign, bool protectEvil, bool protectKilling, bool protectOutlier, bool protectApocalypse)
        {
            var roleOptions = OptionGroupSingleton<RoleOptions>.Instance;
            var minBenign = (int)roleOptions.MinNeutralBenign.Value;
            var minEvil = (int)roleOptions.MinNeutralEvil.Value;
            var minKilling = (int)roleOptions.MinNeutralKiller.Value;
            var minOutlier = (int)roleOptions.MinNeutralOutlier.Value;
            var minApocalypse = (int)OptionGroupSingleton<TownOfUsMiraJK.Options.RoleJKOptions>.Instance.MinNeutralApocalypse.Value;

            if (protectBenign && minBenign < 1)
            {
                minBenign = 1;
            }
            if (protectEvil && minEvil < 1)
            {
                minEvil = 1;
            }
            if (protectKilling && minKilling < 1)
            {
                minKilling = 1;
            }
            if (protectOutlier && minOutlier < 1)
            {
                minOutlier = 1;
            }
            if (protectApocalypse && minApocalypse < 1)
            {
                minApocalypse = 1;
            }

            while (Math.Ceiling((double)crewmateCount / 2) <= nbCount + neCount + nkCount + noCount + naCount)
            {
                var totalNeutrals = nbCount + neCount + nkCount + noCount + naCount;
                if (totalNeutrals == 0)
                {
                    break;
                }

                var factionIndices = new List<int> { 0, 1, 2, 3, 4 };
                factionIndices.Shuffle();

                var canSubtractBenign = nbCount > minBenign;
                var canSubtractEvil = neCount > minEvil;
                var canSubtractKilling = nkCount > minKilling;
                var canSubtractOutlier = noCount > minOutlier;
                var canSubtractApocalypse = naCount > minApocalypse;
                var canSubtractAny = canSubtractBenign || canSubtractEvil || canSubtractKilling || canSubtractOutlier || canSubtractApocalypse;

                bool subtracted = false;
                foreach (var index in factionIndices)
                {
                    switch (index)
                    {
                        case 0 when nbCount > 0 && (canSubtractBenign || !canSubtractAny):
                            nbCount -= 1;
                            subtracted = true;
                            break;
                        case 1 when neCount > 0 && (canSubtractEvil || !canSubtractAny):
                            neCount -= 1;
                            subtracted = true;
                            break;
                        case 2 when nkCount > 0 && (canSubtractKilling || !canSubtractAny):
                            nkCount -= 1;
                            subtracted = true;
                            break;
                        case 3 when noCount > 0 && (canSubtractOutlier || !canSubtractAny):
                            noCount -= 1;
                            subtracted = true;
                            break;
                        case 4 when naCount > 0 && (canSubtractApocalypse || !canSubtractAny):
                            naCount -= 1;
                            subtracted = true;
                            break;
                    }

                    if (subtracted)
                    {
                        break;
                    }
                }

                if (!subtracted)
                {
                    if (nbCount > minBenign)
                    {
                        nbCount -= 1;
                    }
                    else if (neCount > minEvil)
                    {
                        neCount -= 1;
                    }
                    else if (nkCount > minKilling)
                    {
                        nkCount -= 1;
                    }
                    else if (noCount > minOutlier)
                    {
                        noCount -= 1;
                    }
                    else if (naCount > minApocalypse)
                    {
                        naCount -= 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        private static void AddUpRequestedRolesToPools(List<ushort> impRoles, List<ushort> crewRoles)
        {
            var upRequests = UpCommandRequests.GetAllRequests();
            foreach (var (playerName, _) in upRequests)
            {
                if (!UpCommandRequests.TryGetRequestRole(playerName, out var requestedRole))
                {
                    continue;
                }

                var requestedRoleId = (ushort)requestedRole.Role;
                var targetPool = requestedRole.IsImpostor() ? impRoles : crewRoles;

                if (!targetPool.Contains(requestedRoleId))
                {
                    targetPool.Add(requestedRoleId);
                }
            }
        }
        private static void AssignRolesToPlayers(List<PlayerControl> players, List<ushort> roles, string teamName)
        {
            if (roles.Count == 0 || players.Count == 0)
            {
                return;
            }
            Warning($"Assigning {roles.Count} {teamName} roles to {players.Count} players...");

            players.Shuffle();
            roles.Shuffle();

            var upRequestedRoles = new List<(ushort roleId, string playerName)>();
            foreach (var player in players.ToList())
            {
                if (UpCommandRequests.TryGetRequestRole(player.Data.PlayerName, out var requestedRole))
                {
                    var requestedRoleId = (ushort)requestedRole.Role;
                    if (roles.Contains(requestedRoleId) && players.Contains(player))
                    {
                        upRequestedRoles.Add((requestedRoleId, player.Data.PlayerName));
                    }
                }
            }

            upRequestedRoles.Shuffle();

            foreach (var (roleId, playerName) in upRequestedRoles)
            {
                var player = players.FirstOrDefault(p => p.Data.PlayerName == playerName);
                if (player != null && roles.Contains(roleId))
                {
                    AssignRoleToPlayer(player, roleId, true);
                    UpCommandRequests.RemoveRequest(playerName);
                    players.Remove(player);
                    roles.Remove(roleId);
                }
            }

            roles.Shuffle();
            players.Shuffle();

            foreach (var role in roles)
            {
                if (players.Count == 0)
                {
                    break;
                }

                var randomIndex = UnityEngine.Random.RandomRangeInt(0, players.Count);
                var player = players[randomIndex];

                AssignRoleToPlayer(player, role, false);
                players.RemoveAt(randomIndex);
            }
        }
        private static void AssignRoleToPlayer(PlayerControl player, ushort roleId, bool viaUpCommand)
        {
            player.RpcSetRole((RoleTypes)roleId);
            var roleName = RoleManager.Instance.GetRole((RoleTypes)roleId).GetRoleName();
            var source = viaUpCommand ? " (via /up)" : string.Empty;
            var roleText = $"SelectRoles - player: '{player.Data.PlayerName}', role: '{roleName}'{source}";
            MiscUtils.LogInfo(TownOfUsEventHandlers.LogLevel.Warning, roleText);
        }
        private static void AssignVanillaRoles(List<PlayerControl> crewmates, List<PlayerControl> impostors)
        {
            foreach (var player in crewmates)
            {
                player.RpcSetRole(RoleTypes.Crewmate);
            }

            foreach (var player in impostors)
            {
                player.RpcSetRole(RoleTypes.Impostor);
            }
        }
        private static List<RoleListOption> BuildRoleListBuckets(int playerCount)
        {
            var opts = OptionGroupSingleton<RoleOptions>.Instance;
            var buckets = new List<RoleListOption>();
            var slotValues = new[]
            {
            opts.Slot1, opts.Slot2, opts.Slot3, opts.Slot4, opts.Slot5,
            opts.Slot6, opts.Slot7, opts.Slot8, opts.Slot9, opts.Slot10,
            opts.Slot11, opts.Slot12, opts.Slot13, opts.Slot14, opts.Slot15
        };
            var opts2 = OptionGroupSingleton<TownOfUsMiraJK.Options.RoleJKOptions>.Instance;
            var slotReplacements = new[]
            {
            opts2.Slot1, opts2.Slot2, opts2.Slot3, opts2.Slot4, opts2.Slot5,
            opts2.Slot6, opts2.Slot7, opts2.Slot8, opts2.Slot9, opts2.Slot10,
            opts2.Slot11, opts2.Slot12, opts2.Slot13, opts2.Slot14, opts2.Slot15
        };
            var slotsToAdd = Math.Min(playerCount, 15);
            for (var i = 0; i < slotsToAdd; i++)
            {
                if (slotReplacements[i] == Options.RoleListOption.NeutralApocalypse)
                {
                    buckets.Add((RoleListOption)25);
                    continue;
                }
                buckets.Add(slotValues[i].Value);
            }

            if (playerCount > 15)
            {
                for (var i = 0; i < playerCount - 15; i++)
                {
                    var random = UnityEngine.Random.RandomRangeInt(0, 4);
                    buckets.Add(random == 0 ? RoleListOption.CrewRandom : RoleListOption.NonImp);
                }
            }

            return buckets;
        }
        private static void AdjustRoleListBucketsForImpostors(List<RoleListOption> buckets, List<RoleListOption> impBuckets, int requiredImpostors)
        {
            var impCount = buckets.Count(bucket => impBuckets.Contains(bucket));
            var anySlots = buckets.Count(bucket => bucket == RoleListOption.Any);

            while (impCount > requiredImpostors)
            {
                buckets.Shuffle();
                var lastImpIndex = buckets.FindLastIndex(bucket => impBuckets.Contains(bucket));
                if (lastImpIndex >= 0)
                {
                    buckets.RemoveAt(lastImpIndex);
                    buckets.Add(RoleListOption.NonImp);
                    impCount -= 1;
                }
                else
                {
                    break;
                }
            }

            while (impCount + anySlots < requiredImpostors)
            {
                buckets.Shuffle();
                if (buckets.Count > 0)
                {
                    buckets.RemoveAt(0);
                    buckets.Add(RoleListOption.ImpRandom);
                    impCount += 1;
                }
                else
                {
                    break;
                }
            }

            while (buckets.Contains(RoleListOption.Any))
            {
                buckets.Shuffle();
                var anyIndex = buckets.FindLastIndex(bucket => bucket == RoleListOption.Any);
                if (anyIndex >= 0)
                {
                    buckets.RemoveAt(anyIndex);
                    if (impCount < requiredImpostors)
                    {
                        buckets.Add(RoleListOption.ImpRandom);
                        impCount += 1;
                    }
                    else
                    {
                        buckets.Add(RoleListOption.NonImp);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        private static void EnsureCrewNeutralRoles(List<RoleListOption> buckets, List<RoleListOption> impBuckets, List<RoleListOption> crewNkBuckets)
        {
            var hasCrewNeutral = buckets.Any(bucket => crewNkBuckets.Contains(bucket));
            var hasNeutRandom = buckets.Contains(RoleListOption.NeutRandom);
            var hasNonImp = buckets.Contains(RoleListOption.NonImp);

            if (!hasCrewNeutral)
            {
                var replacementOptions = new List<RoleListOption> { RoleListOption.CrewRandom, RoleListOption.NeutKilling };
                replacementOptions.Shuffle();

                if (hasNeutRandom)
                {
                    buckets.Remove(RoleListOption.NeutRandom);
                    buckets.Add(RoleListOption.NeutKilling);
                }
                else if (hasNonImp)
                {
                    buckets.Remove(RoleListOption.NonImp);
                    buckets.Add(replacementOptions[0]);
                }
                else
                {
                    buckets.Shuffle();
                    var nonImpIndex = buckets.FindLastIndex(bucket => !impBuckets.Contains(bucket));
                    if (nonImpIndex >= 0)
                    {
                        buckets.RemoveAt(nonImpIndex);
                        buckets.Add(replacementOptions[0]);
                    }
                }
            }
        }
    }
}
