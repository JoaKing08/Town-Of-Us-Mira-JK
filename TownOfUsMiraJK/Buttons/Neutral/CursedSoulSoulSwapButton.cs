using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Neutral;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules;
using TownOfUs.Modules.Localization;
using TownOfUs.Networking;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modules;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class CursedSoulSoulSwapButton : TownOfUsRoleButton<CursedSoulRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleCursedSoulSoulSwap", "Soul Swap");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => Colors.CursedSoul;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<CursedSoulOptions>.Instance.SoulSwapCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => NeutAssets.CursedSoulSoulSwapSprite;

    public override PlayerControl? GetTarget()
    {return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Cursed Soul Soul Swap: Target is null");
            return;
        }

        var opts = OptionGroupSingleton<CursedSoulOptions>.Instance;
        var rand = UnityEngine.Random.RandomRangeInt(1, 101);
        var randTarget = rand <= opts.RandomSwapChance;
        PlayerControl target;
        if (randTarget && (CursedSoulRole.IsRoleValid(Target.Data.Role) || opts.KillOnNonValidSwap))
        {
            target = Target;
        }
        else
        {
            target = Helpers.GetAlivePlayers().Where(x => !x.AmOwner && x.PlayerId != Target.PlayerId && (CursedSoulRole.IsRoleValid(x.Data.Role) || opts.KillOnNonValidSwap)).Shuffle().FirstOrDefault() ?? Target;
        }
        if (CursedSoulRole.IsRoleValid(target.Data.Role))
        {
            CursedSoulRole.RpcSoulSwap(PlayerControl.LocalPlayer, target);
        }
        else
        {
            PlayerControl.LocalPlayer.RpcSpecialMurder(PlayerControl.LocalPlayer, true, true, resetKillTimer: false, causeOfDeath: "CursedSoul");
        }
    }
}