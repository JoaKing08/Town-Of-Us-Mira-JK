using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers.Crewmate.Coroner;
using TownOfUs.Modules.Localization;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Buttons.Neutral;

public sealed class CoronerAutopsyButton : TownOfUsRoleButton<CoronerRole, DeadBody>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleCoronerAutopsy", "Autopsy");
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override int MaxUses => Target == null ? -1 : (int)OptionGroupSingleton<CoronerOptions>.Instance.MaxAutopsy;
    public override Color TextOutlineColor => Colors.Coroner;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<CoronerOptions>.Instance.AutopsyCooldown + MapCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => CrewAssets.CoronerAutopsySprite;

    public override bool CanUse()
    {
        return base.CanUse() && UsesLeft > 0;
    }
    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Reaper Reap: Target is null");
            return;
        }

        Role.PerformAutopsy(Target);
    }

    public override DeadBody? GetTarget()
    {
        var target = PlayerControl.LocalPlayer == null ? null : Helpers.GetNearestDeadBodies(PlayerControl.LocalPlayer.GetTruePosition(),
            PlayerControl.LocalPlayer.MaxReportDistance / 4f, Helpers.CreateFilter(Constants.NotShipMask))
            .Find(component => component && !component.Reported && Role.CanAutopsy(component.ParentId) &&
            0 < (OptionGroupSingleton<CoronerOptions>.Instance.MaxAutopsy -
            PlayerControl.LocalPlayer?.GetModifiers<CoronerResultBaseModifier>(x => x.Body?.ParentId == component.ParentId).Count()));
        SetUses(target == null ? 0 : (int)OptionGroupSingleton<CoronerOptions>.Instance.MaxAutopsy - PlayerControl.LocalPlayer?.GetModifiers<CoronerResultBaseModifier>(x => x.Body?.ParentId == target.ParentId).Count() ?? 0);
        if (target == null)
        {
            Button?.usesRemainingSprite.gameObject.SetActive(false);
            Button?.usesRemainingText.gameObject.SetActive(false);
        }
        else
        {
            Button?.usesRemainingSprite.gameObject.SetActive(true);
            Button?.usesRemainingText.gameObject.SetActive(true);
        }
        return target;
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return target && target?.Reported == false;
    }
}