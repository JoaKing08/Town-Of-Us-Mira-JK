using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfUs.Buttons;
using TownOfUs.Options;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class BerserkerKillButton : TownOfUsKillRoleButton<BerserkerRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => TranslationController.Instance.GetStringWithDefault(StringNames.KillLabel, "Kill");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => Colors.Berserker;
    public override LoadableAsset<Sprite> Sprite => NeutAssets.BerserkerKillSprite;
    public override float Cooldown => GetCooldown();

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        Coroutines.Start(MiscUtils.CoMoveButtonIndex(this, false));
    }

    public static float BaseCooldown => Math.Clamp(OptionGroupSingleton<BerserkerOptions>.Instance.KillCooldown + MapCooldown, 5f, 120f);

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Berserker Kill: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target, MeetingCheck.OutsideMeeting);
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover() && (!x.IsApocalypseAligned() || OptionGroupSingleton<LoversOptions>.Instance.LoverKillTeammates || !OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam));
        }

        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsApocalypse() || !OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam);
    }

    public static float GetCooldown()
    {
        var berserker = PlayerControl.LocalPlayer.Data.Role as BerserkerRole;

        if (berserker == null)
        {
            return BaseCooldown;
        }

        var options = OptionGroupSingleton<BerserkerOptions>.Instance;

        return Math.Max(BaseCooldown - options.KillCooldownReduction.Value * berserker.KillCount, 0);
    }
}