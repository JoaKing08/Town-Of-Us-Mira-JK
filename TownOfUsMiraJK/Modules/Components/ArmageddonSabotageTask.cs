using AmongUs.Data;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using Reactor.Utilities.Attributes;
using System.Collections;
using TownOfUs.Assets;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Modules.Anims;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Neutral;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsMiraJK.Modules.Components;

[RegisterInIl2Cpp]
public sealed class ArmageddonSabotageTask(nint cppPtr) : PlayerTask(cppPtr)
{
    public override int TaskStep => !IsComplete ? 0 : 1;
    public override bool IsComplete => _isComplete;
    private bool _isComplete;
    private bool _triggeredArmageddon;
    private ArmageddonSabotageSystem _sabotage;
    private Coroutine? _flash;

    public override bool ValidConsole(Console console)
    {
        return false;
    }

    private void FixedUpdate()
    {
        if (IsComplete) return;

        if (!_sabotage.IsActive)
        {
            Complete();
        }
    }

    private float _ogShakeAmt;
    private bool _ogShakeEnabled;
    private float _ogShakePeriod;
    private bool _even;

    public override void Initialize()
    {
        _sabotage = ShipStatus.Instance.Systems[(SystemTypes)ArmageddonSabotageSystem.SabotageId]
            .Cast<ArmageddonSabotageSystem>();
        _flash ??= HudManager.Instance.StartCoroutine(CoFlash().WrapToIl2Cpp());

        _ogShakeEnabled = DataManager.Settings.Gameplay.ScreenShake;
        _ogShakeAmt = HudManager.Instance.PlayerCam.shakeAmount;
        _ogShakePeriod = HudManager.Instance.PlayerCam.shakePeriod;
        DataManager.Settings.Gameplay.ScreenShake = true;
        
        var text = TouLocale.GetParsed("TouJKRoleDeathWarningNotif").Replace("<role>", $"{Colors.Death.ToTextColor()}{TouLocale.Get("TouJKRoleDeath")}</color>");

        var notif1 = Helpers.CreateAndShowNotification(
            text.Replace("<time>", $"{(int)OptionGroupSingleton<ReaperJKOptions>.Instance.ArmageddonTimer}"),
            Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Death.LoadAsset());
        notif1.AdjustNotification();
    }

    [HideFromIl2Cpp]
    private IEnumerator CoFlash()
    {
        HudManager.Instance.FullScreen.color = new Color(Colors.Death.r, Colors.Death.g, Colors.Death.b, 0.34f);
        var wait = new WaitForSeconds(1f);
        var playSound = false;
        while (_sabotage.TimeRemaining > 0)
        {
            var disableBlare = (MeetingHud.Instance != null || ExileController.Instance != null);
            if (_sabotage.Stage == ArmageddonStage.Countdown)
            {
                HudManager.Instance.FullScreen.color = new Color(Colors.Death.r, Colors.Death.g, Colors.Death.b, playSound ? 0.18f : 0.34f);
                HudManager.Instance.FullScreen.gameObject.SetActive(true);
                HudManager.Instance.PlayerCam.shakeAmount = 0.03f;
                HudManager.Instance.PlayerCam.shakePeriod = 16f;

                playSound = !playSound;
                if (playSound && !disableBlare)
                {
                    SoundManager.Instance.StopSound(TouAudio.HexBombAlarmSound.LoadAsset());
                    SoundManager.Instance.PlaySound(TouAudio.HexBombAlarmSound.LoadAsset(), false, 3f);
                }
            }
            else if (_sabotage.Stage == ArmageddonStage.DeathDead)
            {
                if (!ArmageddonSabotageSystem.ArmageddonFinished)
                {
                    HudManager.Instance.FullScreen.color = new Color(Palette.CrewmateBlue.r, Palette.CrewmateBlue.g, Palette.CrewmateBlue.b, playSound ? 0.18f : 0.34f);
                    HudManager.Instance.FullScreen.gameObject.SetActive(true);
                    HudManager.Instance.PlayerCam.shakeAmount = 0f;
                    HudManager.Instance.PlayerCam.shakePeriod = 1f;

                    playSound = !playSound;
                    if (playSound)
                    {
                        SoundManager.Instance.StopSound(TouAudio.HexBombAlarmSound.LoadAsset());
                        SoundManager.Instance.PlaySound(TouAudio.HexBombAlarmSound.LoadAsset(), false, 0.1f);
                    }
                }
            }
            else if (_sabotage.Stage == ArmageddonStage.Finished)
            {
                if (_triggeredArmageddon)
                {
                    // Do nothing
                }
                else
                {
                    var deathAnim = AnimStore.SpawnAnimBody(PlayerControl.LocalPlayer, TouAssets.HexBombDeathPrefab.LoadAsset());
                    var deathBg = Object.Instantiate(HudManager.Instance.FullScreen,
                        deathAnim.transform.GetParent().transform);
                    deathAnim.name = "Disintegrate Animation";
                    deathBg.gameObject.name = "Death Background";
                    deathAnim.SetActive(false);
                    var deathRend = deathAnim.GetComponent<SpriteRenderer>();
                    deathRend.color = new Color(0f, 0f, 0f, 0.17254903f);
                    deathBg.color = new Color(Colors.Death.r, Colors.Death.g, Colors.Death.b, 0.37254903f);
                    deathAnim.transform.localPosition += new Vector3((PlayerControl.LocalPlayer.MyPhysics.FlipX) ? 0f : -0.4f, 0.1f, deathBg.transform.localPosition.z - 100f);
                    deathBg.transform.localScale *= 20f;
                    deathAnim.gameObject.layer = deathBg.gameObject.layer;
                    if (PlayerControl.LocalPlayer.HasModifier<GiantModifier>())
                    {
                        deathAnim.transform.localPosition += new Vector3(0.5f, 0.2f, 0f);
                    }
                    else if (PlayerControl.LocalPlayer.HasModifier<MiniModifier>())
                    {
                        deathAnim.transform.localPosition += new Vector3(0f, -0.05f, 0f);
                    }
                    SoundManager.Instance.StopSound(TouAudio.HexBombAlarmSound.LoadAsset());
                    SoundManager.Instance.PlaySound(TouAudio.HexBombDetonateSound.LoadAsset(), false, 1f);
                    HudManager.Instance.FullScreen.gameObject.SetActive(true);
                    HudManager.Instance.FullScreen.color = new Color(Colors.Death.r, Colors.Death.g, Colors.Death.b, 0.37254903f);
                    deathAnim.SetActive(true);
                    yield return MiscUtils.FadeInDualRenderers(deathBg, deathRend, 0.01f, 0.03f, 5f);
                    yield return new WaitForSeconds(10f);
                    Destroy(deathBg);
                    Destroy(deathAnim);
                }
                _triggeredArmageddon = true;
            }
            else
            {
                HudManager.Instance.FullScreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                if (!HudManager.Instance.FullScreen.gameObject.activeSelf && !disableBlare)
                {
                    SoundManager.Instance.StopSound(TouAudio.HexBombAlarmSound.LoadAsset());
                    SoundManager.Instance.PlaySound(TouAudio.HexBombAlarmSound.LoadAsset(), false, 3f);
                }
                HudManager.Instance.FullScreen.gameObject.SetActive(!HudManager.Instance.FullScreen.gameObject.activeSelf);
            }
            yield return wait;
        }
    }

    public override void AppendTaskText(Il2CppSystem.Text.StringBuilder sb)
    {
        _even = !_even;
        var color = _even ? Color.yellow : Color.red;
        if (_sabotage.Stage == ArmageddonStage.Countdown)
        {
            color = _even ? new Color(0.7f, 0.5f, 0f) : Color.red;
        }

        var text = TouLocale.Get("TouJKRoleDeathArmageddonTriggered");
        switch (_sabotage.Stage)
        {
            case ArmageddonStage.Initiate:
                text = TouLocale.GetParsed("TouJKRoleDeathArmageddonInProgress").Replace("<time>", ((int)_sabotage.TimeRemaining + 1 + (int)_sabotage.duration).ToString());
                break;
            case ArmageddonStage.Countdown:
                text = TouLocale.GetParsed("TouJKRoleDeathArmageddonInProgress").Replace("<time>", ((int)_sabotage.TimeRemaining + 1).ToString());
                break;
            case ArmageddonStage.DeathDead:
                color = Palette.CrewmateBlue;
                text = TouLocale.Get("TouJKRoleDeathArmageddonDeathDead");
                break;
        }

        sb.AppendLine($"{color.ToTextColor()}\n{text}</color>");
    }

    public override void Complete()
    {
        if (_flash != null)
        {
            HudManager.Instance.StopCoroutine(_flash);
            _flash = null;
            HudManager.Instance.FullScreen.gameObject.SetActive(false);
            SoundManager.Instance.StopSound(TouAudio.HexBombAlarmSound.LoadAsset());
        }
        DataManager.Settings.Gameplay.ScreenShake = _ogShakeEnabled;
        HudManager.Instance.PlayerCam.shakeAmount = _ogShakeAmt;
        HudManager.Instance.PlayerCam.shakePeriod = _ogShakePeriod;

        _isComplete = true;
        PlayerControl.LocalPlayer.RemoveTask(this);
    }
}
