using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using System.Collections;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules.Localization;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class AmmitDevourButton : TownOfUsKillRoleButton<AmmitRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleAmmitDevour", "Devour");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => Colors.Ammit;
    public override float Cooldown => BaseCooldown + (OptionGroupSingleton<AmmitOptions>.Instance.DevourCooldownIncrease * Role.TryCast<AmmitRole>()?.Devoured.Count ?? 0);
    public float BaseCooldown => Math.Clamp(OptionGroupSingleton<AmmitOptions>.Instance.DevourCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => NeutAssets.AmmitDevourSprite;
    public override int MaxUses => (int)OptionGroupSingleton<AmmitOptions>.Instance.MaxDevoured;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Ammit Devour: Target is null");
            return;
        }

        Target.RpcAddModifier<AmmitDevouredModifier>(PlayerControl.LocalPlayer);
        Coroutines.Start(CoResetCooldown(this));
    }
    public static IEnumerator CoResetCooldown(AmmitDevourButton button)
    {
        if (button == null)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.01f);
        button.ResetCooldownAndOrEffect();
    }
}