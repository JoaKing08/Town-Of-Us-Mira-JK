using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.LocalSettings;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfUs;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Modifiers.Game.Impostor;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Patches
{
    public static class PlayerRoleTextExtensions
    {

        private static Func<BakerFedModifier, bool> BakerPredicate { get; } =
            brModifier => brModifier.BakerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<TavernKeeperDrunkModifier, bool> TavernKeeperPredicate { get; } =
            tkModifier => tkModifier.TavernKeeperId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<BodyguardGuardModifier, bool> BodyguardPredicate { get; } =
            bgModifier => bgModifier.Bodyguard.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<CrusaderFortifyModifier, bool> CrusaderPredicate { get; } =
            crModifier => crModifier.Crusader.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<GunslingerAimedModifier, bool> GunslingerPredicate { get; } =
            crModifier => crModifier.Gunslinger.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<WitchMarkModifier, bool> WitchPredicate { get; } =
            crModifier => crModifier.Witch.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<PoisonerPoisonModifier, bool> PoisonerPredicate { get; } =
            crModifier => crModifier.Poisoner.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        private static Func<DemagogueImmunityModifier, bool> DemagoguePredicate { get; } =
            diModifier => diModifier.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId || PlayerControl.LocalPlayer.IsImpostorAligned();

        [HarmonyPatch]
        public static class UpdateRoleNameText
        {
            private static readonly Dictionary<byte, Vector3> _colorBlindBasePos = new();

            [HarmonyPatch(typeof(HudManagerPatches), nameof(HudManagerPatches.UpdateRoleNameText))]
            [HarmonyPrefix]
            public static bool Prefix()
            {
                var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                var taskOpt = OptionGroupSingleton<TaskTrackingOptions>.Instance;

                static PlayerControl GetDisguiseTargetOrSelf(PlayerControl player)
                {
                    if (player.TryGetModifier<MorphlingMorphModifier>(out var morph) && morph.Target != null)
                    {
                        return morph.Target;
                    }

                    if (player.TryGetModifier<GlitchMimicModifier>(out var mimic) && mimic.Target != null)
                    {
                        return mimic.Target;
                    }

                    return player;
                }

                static string GetDiedR1ExtraNameTextForDisplayedIdentity(PlayerControl player)
                {
                    var displayPlayer = GetDisguiseTargetOrSelf(player);
                    var mod = displayPlayer.GetModifiers<BaseRevealModifier>()
                        .FirstOrDefault(x => x.Visible && x is FirstRoundIndicator && x.ExtraNameText != string.Empty);
                    return mod?.ExtraNameText ?? string.Empty;
                }

                var colorPlayerNames = LocalSettingsTabSingleton<TownOfUsLocalSettings>.Instance.ColorPlayerNameToggle.Value;
                var localDead = PlayerControl.LocalPlayer.HasDied();
                var localGhost = localDead && genOpt.TheDeadKnow;
                var localImp = PlayerControl.LocalPlayer.IsImpostorAligned() &&
                               genOpt is
                               { ImpsKnowRoles.Value: true, FFAImpostorMode: false } &&
                               !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>();
                var localVamp = PlayerControl.LocalPlayer.GetRoleWhenAlive() is VampireRole;
                var useMiraApiChecks =
                    !localDead && (!PlayerControl.LocalPlayer.IsImpostorAligned() || !(genOpt.FFAImpostorMode || PlayerControl.LocalPlayer.HasModifier<OutcastModifier>()));

                if (MeetingHud.Instance)
                {
                    foreach (var playerVA in MeetingHud.Instance.playerStates)
                    {
                        if (!playerVA.gameObject.active)
                        {
                            continue;
                        }
                        var player = MiscUtils.PlayerById(playerVA.TargetPlayerId);
                        playerVA.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);

                        if (player == null || player.Data == null || player.Data.Role == null)
                        {
                            var data = EndGamePatches.ContainedMeetingData.PlayerMeetingRecords.FirstOrDefault(x => x.PlayerId == playerVA.TargetPlayerId);
                            if (data != null)
                            {
                                EndGamePatches.ContainedMeetingData.DisplayRecordData(playerVA.NameText, data, colorPlayerNames, localGhost);
                            }
                            continue;
                        }

                        var revealMods = player.GetModifiers<BaseRevealModifier>().ToList();

                        var playerName = player.GetDefaultAppearance().PlayerName ?? "Unknown";
                        var playerColor = Color.white;

                        if (colorPlayerNames && PlayerControl.LocalPlayer.IsImpostorAligned() && (player.IsImpostorAligned() || player.IsRole<UndercoverRole>()) &&
                            !player.AmOwner && !genOpt.FFAImpostorMode && !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>() && !player.HasModifier<OutcastModifier>())
                        {
                            playerColor = Color.red;
                        }

                        playerColor = playerColor.UpdateTargetColor(player);
                        playerName = playerName.UpdateTargetSymbols(player);
                        playerName = playerName.UpdateProtectionSymbols(player);
                        playerName = playerName.UpdateAllianceSymbols(player);
                        playerName = playerName.UpdateStatusSymbols(player);

                        var role = player.Data.Role;
                        var customRole = player.Data.Role as ICustomRole;

                        if (role == null)
                        {
                            continue;
                        }

                        var color = role.TeamColor;

                        if (HaunterRole.HaunterVisibilityFlag(player))
                        {
                            playerColor = color;
                        }

                        color = Color.white;

                        var roleName = "";

                        var impostorBuddy = localImp && player.IsImpostorAligned() && !player.HasModifier<OutcastModifier>();
                        var vampBuddy = localVamp && role is VampireRole;
                        var revealed = revealMods.Any(x => x.Visible && x.RevealRole);
                        var localFairy = FairyRole.FairySeesRoleVisibilityFlag(player);
                        var localSleuth = SleuthModifier.SleuthVisibilityFlag(player);
                        var apocFlag = PlayerControl.LocalPlayer.IsApocalypseAligned() && player.IsApocalypseAligned() && OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam;
                        var undeadFlag = (PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<NecromancerRole>()) || PlayerControl.LocalPlayer.HasModifier<NecromancerUndeadModifier>()) && (player.Is((RoleTypes)RoleId.Get<NecromancerRole>()) || player.HasModifier<NecromancerUndeadModifier>());
                        var witchFlag = PlayerControl.LocalPlayer.IsRole<WitchRole>() && player.HasModifier<WitchRevealModifier>() &&
                            !(player.AmOwner || revealed || localGhost || localSleuth || undeadFlag ||
                            useMiraApiChecks && customRole != null && customRole.CanLocalPlayerSeeRole(player));
                        if (player.AmOwner || vampBuddy || impostorBuddy || revealed || localGhost || localFairy || localSleuth || apocFlag || undeadFlag ||
                            witchFlag || useMiraApiChecks && customRole != null && customRole.CanLocalPlayerSeeRole(player) && !player.HasModifier<OutcastModifier>())
                        {
                            color = role.TeamColor;
                            roleName = $"<size=80%>{color.ToTextColor()}{player.Data.Role.GetRoleName()}</color></size>";

                            var revealedRole = revealMods.FirstOrDefault(x => x.Visible && x.RevealRole && x.ShownRole != null);
                            if (revealedRole != null)
                            {
                                color = revealedRole.ShownRole!.TeamColor;
                                roleName =
                                    $"<size=80%>{color.ToTextColor()}{revealedRole.ShownRole!.GetRoleName()}</color></size>";
                            }

                            if (witchFlag)
                            {
                                switch (OptionGroupSingleton<WitchOptions>.Instance.Learns)
                                {
                                    case WitchLearns.Faction:
                                        if (role.IsCrewmate())
                                        {
                                            color = Palette.CrewmateBlue;
                                            roleName =
                                                $"<size=80%>{color.ToTextColor()}{TouLocale.Get("CrewmateKeyword")}</color></size>";
                                        }
                                        else if (role.IsImpostor())
                                        {
                                            color = Palette.ImpostorRed;
                                            roleName =
                                                $"<size=80%>{color.ToTextColor()}{TouLocale.Get("ImpostorKeyword")}</color></size>";
                                        }
                                        else
                                        {
                                            color = TownOfUsColors.Neutral;
                                            roleName =
                                                $"<size=80%>{color.ToTextColor()}{TouLocale.Get("NeutralKeyword")}</color></size>";
                                        }
                                        break;
                                    case WitchLearns.Alignment:
                                        if (role.IsCrewmate())
                                        {
                                            color = Palette.CrewmateBlue;
                                        }
                                        else if (role.IsImpostor())
                                        {
                                            color = Palette.ImpostorRed;
                                        }
                                        else
                                        {
                                            color = TownOfUsColors.Neutral;
                                        }
                                        roleName =
                                            $"<size=80%>{color.ToTextColor()}{MiscUtils.GetParsedRoleAlignment(role)}</color></size>";
                                        break;
                                }
                            }

                            if (!player.HasModifier<VampireBittenModifier>() && role is VampireRole && (vampBuddy || localGhost))
                            {
                                roleName += "<size=80%><color=#FFFFFF> (<color=#A22929>OG</color>)</color></size>";
                            }

                            if (player.HasModifier<AmbassadorRetrainedModifier>() && (impostorBuddy || localGhost))
                            {
                                roleName += "<size=80%><color=#FFFFFF> (<color=#D63F42>Retrained</color>)</color></size>";
                            }

                            var cachedMod = player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole);
                            if (cachedMod is ICachedRole cache && cache.Visible &&
                                player.Data.Role.GetType() != cache.CachedRole.GetType())
                            {
                                var cachedName = cache.CachedRoleName == "" ? cache.CachedRole.GetRoleName() : cache
                                    .CachedRoleName;
                                roleName = cache.ShowCurrentRoleFirst
                                    ? $"<size=80%>{color.ToTextColor()}{player.Data.Role.GetRoleName()}</color> ({cache.CachedRole.TeamColor.ToTextColor()}{cachedName}</color>)</size>"
                                    : $"<size=80%>{cache.CachedRole.TeamColor.ToTextColor()}{cachedName}</color> ({color.ToTextColor()}{player.Data.Role.GetRoleName()}</color>)</size>";
                            }

                            if (player.Data.IsDead && role is GuardianAngelRole gaRole)
                            {
                                roleName = $"<size=80%>{gaRole.TeamColor.ToTextColor()}{TranslationController.Instance.GetString(StringNames.GuardianAngelRole)}</color></size>";
                            }

                            if (localSleuth || (player.Data.IsDead &&
                                                role.Role is RoleTypes.CrewmateGhost
                                                    or RoleTypes.ImpostorGhost))
                            {
                                var roleWhenAlive = player.GetRoleWhenAlive();
                                color = roleWhenAlive.TeamColor;

                                roleName = $"<size=80%>{color.ToTextColor()}{roleWhenAlive.GetRoleName()}</color></size>";
                                if (localDead && !player.HasModifier<VampireBittenModifier>() &&
                                    roleWhenAlive is VampireRole)
                                {
                                    roleName += "<size=80%><color=#FFFFFF> (<color=#A22929>OG</color>)</color></size>";
                                }

                                if (player.HasModifier<AmbassadorRetrainedModifier>() && player.IsImpostorAligned())
                                {
                                    roleName += "<size=80%><color=#FFFFFF> (<color=#D63F42>Retrained</color>)</color></size>";
                                }
                            }

                            if (localDead &&
                                player.TryGetModifier<DeathHandlerModifier>(out var deathMod))
                            {
                                var deathReason =
                                    $"<size=60%>『{Color.yellow.ToTextColor()}{deathMod.CauseOfDeath}</color>』</size>\n";

                                roleName = $"{deathReason}{roleName}";
                            }
                        }

                        var revealedColorMod = revealMods.FirstOrDefault(x => x.Visible && x.NameColor != null);
                        if (revealedColorMod != null)
                        {
                            playerColor = (Color)revealedColorMod.NameColor!;
                            playerName = $"{playerColor.ToTextColor()}{playerName}</color>";
                        }

                        var addedRoleNameText = revealMods.FirstOrDefault(x => x.Visible && x.ExtraRoleText != string.Empty);
                        if (addedRoleNameText != null)
                        {
                            roleName += $"<size=80%>{addedRoleNameText.ExtraRoleText}</size>";
                        }

                        if (((taskOpt.ShowTaskInMeetings && player.AmOwner) ||
                             (localDead && taskOpt.ShowTaskDead)) &&
                            (player.IsCrewmate() || player.Data.Role is SpectreRole))
                        {
                            if (roleName != string.Empty)
                            {
                                roleName += " ";
                            }

                            roleName += $"<size=80%>{player.TaskInfo()}</size>";
                        }

                        if (player.TryGetModifier<OracleConfessModifier>(out var confess, x => x.ConfessToAll))
                        {
                            var accuracy = OptionGroupSingleton<OracleOptions>.Instance.RevealAccuracyPercentage;
                            var revealText = confess.RevealedFaction switch
                            {
                                ModdedRoleTeams.Crewmate =>
                                    $"\n<size=75%>{Palette.CrewmateBlue.ToTextColor()}({accuracy}% Crew) </color></size>",
                                ModdedRoleTeams.Custom =>
                                    $"\n<size=75%>{TownOfUsColors.Neutral.ToTextColor()}({accuracy}% Neut) </color></size>",
                                ModdedRoleTeams.Impostor =>
                                    $"\n<size=75%>{TownOfUsColors.ImpSoft.ToTextColor()}({accuracy}% Imp) </color></size>",
                                _ => string.Empty
                            };

                            playerName += revealText;
                        }

                        var addedPlayerNameText = revealMods.FirstOrDefault(x =>
                            x.Visible && x.ExtraNameText != string.Empty && x is not FirstRoundIndicator);
                        if (addedPlayerNameText != null)
                        {
                            playerName += addedPlayerNameText.ExtraNameText;
                        }

                        var diedR1Text = GetDiedR1ExtraNameTextForDisplayedIdentity(player);
                        if (!string.IsNullOrEmpty(diedR1Text))
                        {
                            playerName += diedR1Text;
                        }

                        if (player.Data?.Disconnected == true)
                        {
                            EndGamePatches.ContainedMeetingData.AddPlayerData(player);
                            // don't wanna leak info!
                            continue;
                        }

                        if (!string.IsNullOrEmpty(roleName))
                        {
                            if (colorPlayerNames)
                            {
                                playerName = $"{roleName}\n{color.ToTextColor()}<size=92%>{playerName}</size></color>";
                            }
                            else
                            {
                                playerName = $"{roleName}\n<size=92%>{playerName}</size>";
                            }
                        }

                        playerVA.NameText.text = playerName;
                        playerVA.NameText.color = playerColor;
                    }
                }
                else
                {
                    var isVisible = (PlayerControl.LocalPlayer.TryGetModifier<DeathHandlerModifier>(out var deathHandler) &&
                                     !deathHandler.DiedThisRound) || TutorialManager.InstanceExists;
                    if (localGhost)
                    {
                        localGhost = isVisible;
                    }
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player == null || player.Data == null || player.Data.Role == null)
                        {
                            continue;
                        }

                        var revealMods = player.GetModifiers<BaseRevealModifier>().ToList();

                        var playerName = player.GetAppearance().PlayerName ?? "Unknown";
                        var playerColor = Color.white;

                        if (colorPlayerNames && PlayerControl.LocalPlayer.IsImpostorAligned() && (player.IsImpostorAligned() || player.IsRole<UndercoverRole>()) &&
                            !player.AmOwner && !genOpt.FFAImpostorMode && !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>() && !player.HasModifier<OutcastModifier>())
                        {
                            playerColor = Color.red;
                        }

                        playerColor = playerColor.UpdateTargetColor(player, !isVisible);
                        playerName = playerName.UpdateTargetSymbols(player, !isVisible);
                        playerName = playerName.UpdateProtectionSymbols(player, !isVisible);
                        playerName = playerName.UpdateAllianceSymbols(player, !isVisible);
                        playerName = playerName.UpdateStatusSymbols(player, !isVisible);

                        var role = player.Data.Role;
                        var customRole = player.Data.Role as ICustomRole;
                        var color = Color.white;

                        if (role == null)
                        {
                            continue;
                        }

                        var roleName = "";
                        var canSeeDeathReason = false;
                        var impostorBuddy = localImp && player.IsImpostorAligned() && !player.HasModifier<OutcastModifier>();
                        var vampBuddy = localVamp && role is VampireRole;
                        var revealed = revealMods.Any(x => x.Visible && x.RevealRole);
                        var localFairy = FairyRole.FairySeesRoleVisibilityFlag(player);
                        var localSleuth = SleuthModifier.SleuthVisibilityFlag(player);
                        var apocFlag = PlayerControl.LocalPlayer.IsApocalypseAligned() && player.IsApocalypseAligned() && OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam;
                        var undeadFlag = (PlayerControl.LocalPlayer.Is((RoleTypes)RoleId.Get<NecromancerRole>()) || PlayerControl.LocalPlayer.HasModifier<NecromancerUndeadModifier>()) && (player.Is((RoleTypes)RoleId.Get<NecromancerRole>()) || player.HasModifier<NecromancerUndeadModifier>());
                        var witchFlag = PlayerControl.LocalPlayer.IsRole<WitchRole>() && player.HasModifier<WitchRevealModifier>() &&
                            !(player.AmOwner || revealed || localGhost || localSleuth || undeadFlag ||
                            useMiraApiChecks && customRole != null && customRole.CanLocalPlayerSeeRole(player));
                        if (player.AmOwner || vampBuddy || impostorBuddy || revealed || localGhost || localFairy || localSleuth || apocFlag || undeadFlag ||
                            witchFlag || useMiraApiChecks && customRole != null && customRole.CanLocalPlayerSeeRole(player) && !player.HasModifier<OutcastModifier>())
                        {
                            color = role.TeamColor;
                            roleName = $"<size=80%>{color.ToTextColor()}{player.Data.Role.GetRoleName()}</color></size>";

                            var revealedRole = revealMods.FirstOrDefault(x => x.Visible && x.RevealRole && x.ShownRole != null);
                            if (revealedRole != null)
                            {
                                color = revealedRole.ShownRole!.TeamColor;
                                roleName =
                                    $"<size=80%>{color.ToTextColor()}{revealedRole.ShownRole!.GetRoleName()}</color></size>";
                            }

                            if (witchFlag)
                            {
                                switch (OptionGroupSingleton<WitchOptions>.Instance.Learns)
                                {
                                    case WitchLearns.Faction:
                                        if (role.IsCrewmate())
                                        {
                                            color = Palette.CrewmateBlue;
                                            roleName =
                                                $"<size=80%>{color.ToTextColor()}{TouLocale.Get("CrewmateKeyword")}</color></size>";
                                        }
                                        else if (role.IsImpostor())
                                        {
                                            color = Palette.ImpostorRed;
                                            roleName =
                                                $"<size=80%>{color.ToTextColor()}{TouLocale.Get("ImpostorKeyword")}</color></size>";
                                        }
                                        else
                                        {
                                            color = TownOfUsColors.Neutral;
                                            roleName =
                                                $"<size=80%>{color.ToTextColor()}{TouLocale.Get("NeutralKeyword")}</color></size>";
                                        }
                                        break;
                                    case WitchLearns.Alignment:
                                        if (role.IsCrewmate())
                                        {
                                            color = Palette.CrewmateBlue;
                                        }
                                        else if (role.IsImpostor())
                                        {
                                            color = Palette.ImpostorRed;
                                        }
                                        else
                                        {
                                            color = TownOfUsColors.Neutral;
                                        }
                                        roleName =
                                            $"<size=80%>{color.ToTextColor()}{MiscUtils.GetParsedRoleAlignment(role)}</color></size>";
                                        break;
                                }
                            }

                            if (!player.HasModifier<VampireBittenModifier>() && role is VampireRole && (vampBuddy || localGhost))
                            {
                                roleName += "<size=80%><color=#FFFFFF> (<color=#A22929>OG</color>)</color></size>";
                            }

                            if (player.HasModifier<AmbassadorRetrainedModifier>() && (impostorBuddy || localGhost))
                            {
                                roleName += "<size=80%><color=#FFFFFF> (<color=#D63F42>Retrained</color>)</color></size>";
                            }

                            var cachedMod = player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole);
                            if (cachedMod is ICachedRole cache && cache.Visible &&
                                player.Data.Role.GetType() != cache.CachedRole.GetType())
                            {
                                var cachedName = cache.CachedRoleName == "" ? cache.CachedRole.GetRoleName() : cache
                                    .CachedRoleName;
                                roleName = cache.ShowCurrentRoleFirst
                                    ? $"<size=80%>{color.ToTextColor()}{player.Data.Role.GetRoleName()}</color> ({cache.CachedRole.TeamColor.ToTextColor()}{cachedName}</color>)</size>"
                                    : $"<size=80%>{cache.CachedRole.TeamColor.ToTextColor()}{cachedName}</color> ({color.ToTextColor()}{player.Data.Role.GetRoleName()}</color>)</size>";
                            }

                            if (player.Data.IsDead && role is GuardianAngelRole gaRole)
                            {
                                roleName = $"<size=80%>{gaRole.TeamColor.ToTextColor()}{TranslationController.Instance.GetString(StringNames.GuardianAngelRole)}</color></size>";
                            }

                            if (localSleuth || (player.Data.IsDead &&
                                                role.Role is RoleTypes.CrewmateGhost
                                                    or RoleTypes.ImpostorGhost))
                            {
                                var roleWhenAlive = player.GetRoleWhenAlive();
                                color = roleWhenAlive.TeamColor;

                                roleName = $"<size=80%>{color.ToTextColor()}{roleWhenAlive.GetRoleName()}</color></size>";
                                if (!player.HasModifier<VampireBittenModifier>() && roleWhenAlive is VampireRole)
                                {
                                    roleName += "<size=80%><color=#FFFFFF> (<color=#A22929>OG</color>)</color></size>";
                                }

                                if (player.HasModifier<AmbassadorRetrainedModifier>() && player.IsImpostorAligned())
                                {
                                    roleName += "<size=80%><color=#FFFFFF> (<color=#D63F42>Retrained</color>)</color></size>";
                                }
                            }

                            if (localDead && isVisible &&
                                player.TryGetModifier<DeathHandlerModifier>(out var deathMod))
                            {
                                var deathReason =
                                    $"<size=75%>『{Color.yellow.ToTextColor()}{deathMod.CauseOfDeath}</color>』</size>\n";

                                roleName = $"{deathReason}{roleName}";
                                canSeeDeathReason = true;
                            }
                        }

                        var revealedColorMod = revealMods.FirstOrDefault(x => x.Visible && x.NameColor != null);
                        if (revealedColorMod != null)
                        {
                            playerColor = (Color)revealedColorMod.NameColor!;
                            playerName = $"{playerColor.ToTextColor()}{playerName}</color>";
                        }

                        var addedRoleNameText = revealMods.FirstOrDefault(x => x.Visible && x.ExtraRoleText != string.Empty);
                        if (addedRoleNameText != null)
                        {
                            roleName += $"<size=80%>{addedRoleNameText.ExtraRoleText}</size>";
                        }

                        if (((taskOpt.ShowTaskRound && player.AmOwner) || (localDead &&
                                                                           taskOpt.ShowTaskDead && isVisible)) &&
                            (player.IsCrewmate() ||
                             player.Data.Role is SpectreRole))
                        {
                            if (roleName != string.Empty)
                            {
                                roleName += " ";
                            }

                            roleName += $"<size=80%>{player.TaskInfo()}</size>";
                        }

                        if (player.AmOwner && player.TryGetModifier<ScatterModifier>(out var scatter) && !player.HasDied())
                        {
                            roleName += $" - {scatter.GetDescription()}";
                        }

                        var addedPlayerNameText = revealMods.FirstOrDefault(x =>
                            x.Visible && x.ExtraNameText != string.Empty && x is not FirstRoundIndicator);
                        if (addedPlayerNameText != null)
                        {
                            playerName += addedPlayerNameText.ExtraNameText;
                        }

                        var diedR1Text = GetDiedR1ExtraNameTextForDisplayedIdentity(player);
                        if (!string.IsNullOrEmpty(diedR1Text))
                        {
                            playerName += diedR1Text;
                        }

                        if (canSeeDeathReason)
                        {
                            playerName += $"\n<size=75%> </size>";
                        }

                        if (player.AmOwner && player.Data.Role is IGhostRole { GhostActive: true })
                        {
                            playerColor = Color.clear;
                        }

                        if (!string.IsNullOrEmpty(roleName))
                        {
                            playerName = colorPlayerNames
                                ? $"{roleName}\n{color.ToTextColor()}{playerName}</color>"
                                : $"{roleName}\n{playerName}";
                        }

                        player.cosmetics.nameText.text = playerName;
                        player.cosmetics.nameText.color = playerColor;

                        player.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0.15f, -0.5f);

                        var cbId = player.PlayerId;
                        var cbCurrent = player.cosmetics.colorBlindText.transform.localPosition;
                        var cbOffset = Vector3.down * 0.12f;

                        if (!_colorBlindBasePos.TryGetValue(cbId, out var cbBase))
                        {
                            cbBase = string.IsNullOrEmpty(diedR1Text) ? cbCurrent : cbCurrent - cbOffset;
                            _colorBlindBasePos[cbId] = cbBase;
                        }
                        else if (string.IsNullOrEmpty(diedR1Text))
                        {
                            var cbExpectedNoR1 = cbBase;
                            var cbExpectedR1 = cbBase + cbOffset;
                            if ((cbCurrent - cbExpectedNoR1).sqrMagnitude > 0.0001f &&
                                (cbCurrent - cbExpectedR1).sqrMagnitude > 0.0001f)
                            {
                                cbBase = cbCurrent;
                                _colorBlindBasePos[cbId] = cbBase;
                            }
                        }

                        player.cosmetics.colorBlindText.transform.localPosition =
                            string.IsNullOrEmpty(diedR1Text) ? cbBase : cbBase + cbOffset;
                    }
                }

                if (HudManager.Instance.TaskPanel != null)
                {
                    var tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP")
                        .GetComponent<TextMeshPro>();
                    tabText.SetText($"{HudManagerPatches.StoredTasksText} {PlayerControl.LocalPlayer.TaskInfo()}");
                }
                return false;
            }
        }

        [HarmonyPatch]
        public static class UpdateTargetSymbols
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateTargetSymbols), new Type[] { typeof(string), typeof(PlayerControl), typeof(DataVisibility) })]
            [HarmonyPostfix]
            public static void Postfix(PlayerControl player, DataVisibility visibility, ref string __result)
            {
                var hidden = visibility == DataVisibility.Hidden;
                var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                var isDead = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden;

                if (player.TryGetModifier<ManhunterTargetModifier>(out var mhmod) && (PlayerControl.LocalPlayer.IsRole<ManhunterRole>() || isDead))
                {
                    var color = Colors.Manhunter;
                    if (!mhmod.KilledByManhunter && player.HasDied())
                    {
                        color = Color.Lerp(Colors.Manhunter, Color.red, 0.75f);
                    }
                    __result += $"<color=#{color.ToHtmlStringRGBA()}> /</color>";
                }
            }
        }

        [HarmonyPatch]
        public static class UpdateAllianceSymbols
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateAllianceSymbols), new Type[] { typeof(string), typeof(PlayerControl), typeof(DataVisibility) })]
            [HarmonyPostfix]
            public static void Postfix(PlayerControl player, DataVisibility visibility, ref string __result)
            {
                var hidden = visibility == DataVisibility.Hidden;
                var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                var isDead = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden;
                if (player.IsCrewmate() && player.TryGetModifier<ProphetModifier>(out var prophetMod) && (ProphetModifier.ProphetVisibilityFlag(player) || isDead))
                {
                    __result += $"<color=#FFFFFF> (<color=#{Colors.Apocalypse.ToHtmlStringRGBA()}>{prophetMod.ShortName}</color>)</color>";
                }
                if (player.TryGetModifier<NecromancerUndeadModifier>(out var undeadMod) && (NecromancerUndeadModifier.UndeadVisibilityFlag(player) || isDead))
                {
                    __result += $"<color=#FFFFFF> (<color=#{Colors.Necromancer.ToHtmlStringRGBA()}>{undeadMod.ShortName}</color>)</color>";
                }
                if (player.Is((RoleTypes)RoleId.Get<UndercoverRole>()) && isDead)
                {
                    __result += $"<color=#{Colors.Undercover.ToHtmlStringRGBA()}> €</color>";
                }
            }
        }

        [HarmonyPatch]
        public static class UpdateStatusSymbols
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateStatusSymbols), new Type[] { typeof(string), typeof(PlayerControl), typeof(DataVisibility) })]
            [HarmonyPostfix]
            public static void Postfix(PlayerControl player, DataVisibility visibility, ref string __result)
            {
                var hidden = visibility == DataVisibility.Hidden;
                var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                var isImp = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.IsImpostor() && genOpt.ImpsKnowRoles && !genOpt.FFAImpostorMode;
                var isDead = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden;

                if (player.HasModifier(BakerPredicate) && (PlayerControl.LocalPlayer.IsRole<BakerRole>() || PlayerControl.LocalPlayer.IsRole<FamineRole>())
                    || player.HasModifier<BakerFedModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Baker.ToHtmlStringRGBA()}> ";
                    for (int i = 0; i < player.GetModifier<BakerFedModifier>().BreadLeft; i++)
                    {
                        __result += "ß";
                    }
                    __result += "</color>";
                }
                if (player.HasModifier(TavernKeeperPredicate) && PlayerControl.LocalPlayer.IsRole<TavernKeeperRole>()
                    || player.HasModifier<TavernKeeperDrunkModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.TavernKeeper.ToHtmlStringRGBA()}> ø</color>";
                }
                if (player.HasModifier(GunslingerPredicate) && PlayerControl.LocalPlayer.IsRole<GunslingerRole>()
                    || player.HasModifier<GunslingerAimedModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Gunslinger.ToHtmlStringRGBA()}> ‡</color>";
                }
                if (player.HasModifier(WitchPredicate) && PlayerControl.LocalPlayer.IsRole<WitchRole>()
                    || player.HasModifier<WitchMarkModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Witch.ToHtmlStringRGBA()}> ð</color>";
                }
                if (player.HasModifier(PoisonerPredicate) && PlayerControl.LocalPlayer.IsRole<PoisonerRole>()
                    || player.HasModifier<PoisonerPoisonModifier>() && isDead)
                {
                    __result += $"<color=#{TownOfUsColors.Impostor.ToHtmlStringRGBA()}> ž</color>";
                }
                if (player.HasModifier(DemagoguePredicate)
                    || player.HasModifier<DemagogueImmunityModifier>() && isDead)
                {
                    __result += $"<color=#{TownOfUsColors.Impostor.ToHtmlStringRGBA()}> Œ</color>";
                }
            }
        }

        [HarmonyPatch]
        public static class UpdateProtectionSymbols
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateProtectionSymbols), new Type[] { typeof(string), typeof(PlayerControl), typeof(DataVisibility) })]
            [HarmonyPostfix]
            public static void Postfix(PlayerControl player, DataVisibility visibility, ref string __result)
            {
                var hidden = visibility == DataVisibility.Hidden;
                var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                var isDead = visibility is DataVisibility.Show || PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden;
                if (player.HasModifier(BodyguardPredicate) && PlayerControl.LocalPlayer.IsRole<BodyguardRole>()
                    || player.HasModifier<BodyguardGuardModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Bodyguard.ToHtmlStringRGBA()}> ()</color>";
                }
                if (player.HasModifier(CrusaderPredicate) && PlayerControl.LocalPlayer.IsRole<CrusaderRole>()
                    || player.HasModifier<CrusaderFortifyModifier>() && isDead)
                {
                    __result += $"<color=#{Colors.Crusader.ToHtmlStringRGBA()}> {{}}</color>";
                }
            }
        }

        [HarmonyPatch]
        public static class UpdateTargetColor
        {
            [HarmonyPatch(typeof(TownOfUs.Utilities.PlayerRoleTextExtensions), nameof(TownOfUs.Utilities.PlayerRoleTextExtensions.UpdateTargetColor), new Type[] { typeof(Color), typeof(PlayerControl), typeof(bool) })]
            [HarmonyPostfix]
            public static void Postfix(PlayerControl player, ref Color __result)
            {
                if (player.IsCrewmate() && !player.HasModifier<AllianceGameModifier>() && PlayerControl.LocalPlayer.IsRole<AnarchistRole>() && OptionGroupSingleton<AnarchistOptions>.Instance.LearnCrew)
                {
                    __result = Palette.CrewmateBlue;
                }
            }
        }
    }
}
