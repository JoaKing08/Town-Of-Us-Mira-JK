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
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Neutral;

public sealed class PsychopathInsanityModifier() : TimedModifier
{
    public override string ModifierName => "Insanity";
    public override float Duration => OptionGroupSingleton<PsychopathOptions>.Instance.InsanityDuration;
    public override bool AutoStart => true;

    public override bool HideOnUi => true;
    public bool NoEndNotif { get; set; }
    public SpriteRenderer Tint { get; set; }
    public bool IsDestroyed { get; set; }


    public override void OnActivate()
    {
        if (Player.AmOwner)
        {
            Tint = GameObject.Instantiate(HudManager.Instance.FullScreen, HudManager.Instance.FullScreen.transform.parent);
            Tint.gameObject.SetActive(true);
            Tint.color = Palette.ImpostorRed.SetAlpha(0.2f);
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRolePsychopathInsanityNotif"),
                Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Psychopath.LoadAsset());

            notif1.AdjustNotification();
        }
        Player.SetKillTimer(Mathf.Min(OptionGroupSingleton<PsychopathOptions>.Instance.InsanityKillCooldown, Player.killTimer));
        Player.MyPhysics.SetForcedBodyType(Player.MyPhysics.bodyType == PlayerBodyTypes.Long ? PlayerBodyTypes.LongSeeker : PlayerBodyTypes.Seeker);
    }

    public override void OnDeactivate()
    {
        IsDestroyed = true;
        if (Player.AmOwner)
        {
            if (!NoEndNotif)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    TouLocale.GetParsed("TouJKRolePsychopathInsanityEndNotif"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: ToUJKRoleIcons.Psychopath.LoadAsset());

                notif1.AdjustNotification();
                Coroutines.Start(CoEndFlash(Tint));
            }
            else
            {
                Tint?.gameObject?.Destroy();
            }
        }
        Player.SetKillTimer(Player.GetKillCooldown());
        Player.MyPhysics.SetForcedBodyType(Player.BodyType);
    }
    public static IEnumerator CoEndFlash(SpriteRenderer tint)
    {
        tint.color = new Color(Palette.CrewmateBlue.r, Palette.CrewmateBlue.g, Palette.CrewmateBlue.b, 0.4f);
        yield return new WaitForSeconds(1f);
        tint?.gameObject?.Destroy();
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