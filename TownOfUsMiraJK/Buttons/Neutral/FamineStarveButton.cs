using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class FamineStarveButton : TownOfUsRoleButton<FamineRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleFamineStarve", "Starve");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => Colors.Famine;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<BakerOptions>.Instance.StarveCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => NeutAssets.FamineStarveSprite;

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: plr => (!plr.IsApocalypseAligned() || !OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam) && !plr.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: plr => (!plr.IsApocalypseAligned() || !OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam));
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && (!target!.IsApocalypseAligned() || !OptionGroupSingleton<GeneralJKOptions>.Instance.ApocTeam);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Famine Starve: Target is null");
            return;
        }

        FamineRole.RpcStarve(PlayerControl.LocalPlayer, Target);
    }
}