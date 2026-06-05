using AmongUs.GameOptions;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Events;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Crewmate;

public sealed class MonsterHunterStakeButton : TownOfUsKillRoleButton<MonsterHunterRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleMonsterHunterStake", "Stake");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfUsMiraJKColors.MonsterHunter;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<MonsterHunterOptions>.Instance.StakeCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => ToUJKCrewAssets.MonsterHunterStakeSprite;
    public override int MaxUses => (int)OptionGroupSingleton<MonsterHunterOptions>.Instance.MaxStakes;

    public static bool Usable =>
        OptionGroupSingleton<MonsterHunterOptions>.Instance.StakeRoundOne || TutorialManager.InstanceExists || DeathEventHandlers.CurrentRound > 1;

    public override bool CanUse()
    {
        return base.CanUse() && Usable && UsesLeft > 0;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Monster Hunter Stake: Target is null");
            return;
        }

        var targetRole = Target.Data.Role;
        var stakeFlag = targetRole.Role == (RoleTypes)RoleId.Get<VampireRole>() || targetRole.Role == (RoleTypes)RoleId.Get<WerewolfRole>() || Target.HasModifier<NecromancerUndeadModifier>();
        if (stakeFlag)
        {
            UsesLeft += 1;
            Button.SetUsesRemaining(UsesLeft);
            Helpers.CreateAndShowNotification($"<color={TownOfUsColors.ImpSoft.ToTextColor()}><b>{TouLocale.GetParsed("TouJKRoleMonsterHunterCorrectStake").Replace("<player>", Target.Data.PlayerName)}</b></color>", Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.MonsterHunter.LoadAsset());

            if (Target.HasModifier<FirstDeadShield>())
            {
                return;
            }

            if (Target.HasModifier<BaseShieldModifier>())
            {
                return;
            }

            PlayerControl.LocalPlayer.RpcCustomMurder(Target, MeetingCheck.OutsideMeeting);
        }
        else
        {
            var touAbilityEvent = new TouAbilityEvent((AbilityType)JKAbilityType.MonsterHunsterIncorrectStake, Role.Player!, Target);
            MiraEventManager.InvokeEvent(touAbilityEvent);
            Helpers.CreateAndShowNotification($"{Palette.CrewmateBlue.ToTextColor()}<b>{TouLocale.GetParsed("TouJKRoleMonsterHunterWrongStake").Replace("<player>", Target.Data.PlayerName)}</b></color>", Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.MonsterHunter.LoadAsset());
            MonsterHunterRole.RpcWrongStake(Role.Player, UsesLeft > 0);
            if (UsesLeft <= 0 && OptionGroupSingleton<MonsterHunterOptions>.Instance.Suicide)
            {
                PlayerControl.LocalPlayer.RpcCustomMurder(PlayerControl.LocalPlayer, MeetingCheck.OutsideMeeting);
            }
        }
        SetTimer(Cooldown);
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }

        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}