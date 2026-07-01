using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities;
using System.Collections;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Neutral;
using TownOfUsMiraJK.Roles.Neutral;
using UnityEngine;

namespace TownOfUs.Modifiers.Neutral;

public sealed class AlchemistPotionModifier(byte alchemist, uint id) : TimedModifier
{
    public override string ModifierName => "To Apply Potion";
    public Type ToApply { get; set; } = ModifierManager.GetModifierType(id);
    public bool Apply { get; set; } = true;
    public PlayerControl Alchemist { get; set; } = MiscUtils.PlayerById(alchemist);
    public override float Duration => OptionGroupSingleton<AlchemistOptions>.Instance.PotionDelay;

    public override void OnActivate()
    {
        if (Alchemist.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed($"TouJKRoleAlchemistPotionNotif")
                .Replace("<player>", $"{TownOfUsMiraJKColors.Alchemist.ToTextColor()}{Player.Data.PlayerName}</color>")
                .Replace("<time>", ((int)OptionGroupSingleton<AlchemistOptions>.Instance.PotionDelay).ToString())
                .Replace("<potion>", $"{TownOfUsMiraJKColors.Alchemist.ToTextColor()}{(ModifierManager.Modifiers.FirstOrDefault(x => x.GetType() == ToApply) as IPotionEffect)?.PotionName ?? TouLocale.Get($"TouJKRoleAlchemistPotionNone")}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Alchemist.LoadAsset());

            notif1.AdjustNotification();
        }
    }
    public override void OnDeactivate()
    {
        if (Apply)
        {
            Player?.AddModifier(ToApply, Alchemist);
            if (Alchemist.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed($"TouJKRoleAlchemistPotionRealNotif")
                    .Replace("<player>", $"{TownOfUsMiraJKColors.Alchemist.ToTextColor()}{Player.Data.PlayerName}</color>")
                    .Replace("<potion>", $"{TownOfUsMiraJKColors.Alchemist.ToTextColor()}{(ModifierManager.Modifiers.FirstOrDefault(x => x.GetType() == ToApply) as IPotionEffect)?.PotionName ?? TouLocale.Get($"TouJKRoleAlchemistPotionNone")}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Alchemist.LoadAsset());

                notif1.AdjustNotification();
            }
            else if (Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed($"TouJKRoleAlchemistPotionTargetNotif")
                    .Replace("<role>", $"{TownOfUsMiraJKColors.Alchemist.ToTextColor()}{Alchemist.Data.Role.GetRoleName()}</color>")
                    .Replace("<potion>", $"{TownOfUsMiraJKColors.Alchemist.ToTextColor()}{(ModifierManager.Modifiers.FirstOrDefault(x => x.GetType() == ToApply) as IPotionEffect)?.PotionName ?? TouLocale.Get($"TouJKRoleAlchemistPotionNone")}</color>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Alchemist.LoadAsset());

                notif1.AdjustNotification();
            }
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        Apply = false;
        ModifierComponent?.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        Apply = false;
        ModifierComponent?.RemoveModifier(this);
    }
}
public interface IPotionEffect
{
    public string PotionName { get; }
    public bool CanAppear { get; }
    public bool CanSelf { get; }
    public PlayerControl Alchemist { get; }
}