using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfUs.Buttons;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modifiers;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class ManhunterKillButton : TownOfUsKillRoleButton<ManhunterRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => TranslationController.Instance.GetStringWithDefault(StringNames.KillLabel, "Kill");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => Colors.Manhunter;
    public override LoadableAsset<Sprite> Sprite => NeutAssets.ManhunterKillSprite;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ManhunterOptions>.Instance.KillCooldown + MapCooldown, 5f, 120f);

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        Coroutines.Start(MiscUtils.CoMoveButtonIndex(this, false));
    }

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Manhunter Kill: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target, MeetingCheck.OutsideMeeting);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => x.HasModifier<ManhunterTargetModifier>());
    }
}