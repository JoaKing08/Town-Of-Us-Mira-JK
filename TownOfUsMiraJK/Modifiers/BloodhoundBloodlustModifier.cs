using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System.Collections;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Buttons.Neutral;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Neutral;

public sealed class BloodhoundBloodlustModifier() : TimedModifier
{
    public override string ModifierName => "Bloodlust";
    public override float Duration => OptionGroupSingleton<BloodhoundOptions>.Instance.BloodlustDuration;
    public override bool AutoStart => true;

    public override bool HideOnUi => true;
    public bool NoEndNotif = false;
    public SpriteRenderer Tint;
    public bool IsDestroyed = false;


    public override void OnActivate()
    {
        if (Player.AmOwner)
        {
            Tint = GameObject.Instantiate(HudManager.Instance.FullScreen, HudManager.Instance.FullScreen.transform.parent);
            Tint.gameObject.SetActive(true);
            Tint.color = new Color(TownOfUsMiraJKColors.Bloodhound.r, TownOfUsMiraJKColors.Bloodhound.g, TownOfUsMiraJKColors.Bloodhound.b, 0.2f);
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleBloodhoundBloodlustNotif"),
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Bloodhound.LoadAsset());

            notif1.AdjustNotification();
            CustomButtonSingleton<BloodhoundKillButton>.Instance.SetTimer(OptionGroupSingleton<BloodhoundOptions>.Instance.BloodlustCooldown);
        }
    }

    public override void OnDeactivate()
    {
        IsDestroyed = true;
        if (Player.AmOwner)
        {
            if (!NoEndNotif)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRoleBloodhoundBloodlustEndNotif"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Bloodhound.LoadAsset());

                notif1.AdjustNotification();
                Coroutines.Start(CoEndFlash(Tint));
            }
            else
            {
                Tint.Destroy();
            }
            CustomButtonSingleton<BloodhoundKillButton>.Instance.ResetCooldownAndOrEffect();
        }
    }
    public static IEnumerator CoEndFlash(SpriteRenderer tint)
    {
        tint.color = new Color(Palette.CrewmateBlue.r, Palette.CrewmateBlue.g, Palette.CrewmateBlue.b, 0.4f);
        yield return new WaitForSeconds(1f);
        tint.Destroy();
    }

    public override void OnDeath(DeathReason reason)
    {
        NoEndNotif = true;
        ModifierComponent?.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        NoEndNotif = true;
        ModifierComponent?.RemoveModifier(this);
    }
}