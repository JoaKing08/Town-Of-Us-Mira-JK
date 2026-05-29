using HarmonyLib;
using UnityEngine;
using TownOfUsMiraJK.Assets;
using Reactor.Utilities.Extensions;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using MiraAPI.GameOptions;

public class SanctifierCircle
{
    public static List<SanctifierCircle> SanctifierCircles { get; set; } = new();
    public float RotationSpeed { get; set; } = -10f;
    public float MinimumAlpha { get; set; } = 0f;
    public float MaximumAlpha { get; set; } = 1f;
    public float CycleDuration { get; set; } = 10f;
    public SpriteRenderer Renderer { get; set; }
    public GameObject GameObject { get; set; }
    private float _timer;
    private bool _fading;
    private float _scale;
    private float _showTimer = 0;
    private bool _shown;
    public void Start()
    {
        _timer = CycleDuration / 2;
        _fading = true;
    }
    public void Update()
    {
        if (GameObject == null)
        {
            Destroy();
            return;
        }
        if (!_shown)
        {
            _showTimer += Time.deltaTime;
            if (_showTimer >= OptionGroupSingleton<SanctifierOptions>.Instance.ShowSanctifyDelay.Value)
            {
                Renderer.enabled = true;
                _shown = true;
            }
        }
        GameObject.transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _timer = CycleDuration / 2;
            _fading = !_fading;
        }
        if (Renderer.enabled)
        {
            var t = _timer * 2 / CycleDuration;
            var color = Renderer.color;
            color.a = Mathf.Lerp(MinimumAlpha, MaximumAlpha, _fading ? t : 1f - t);
            Renderer.color = color;
        }
    }
    public static SanctifierCircle Create(Vector3 position, Transform parent = null, float scale = 1f)
    {
        var gameObject = GameObject.Instantiate(ToUJKAssets.SanctifierCircle.LoadAsset());
        if (parent != null)
        {
            gameObject.transform.SetParent(parent);
        }
        gameObject.transform.localScale *= scale;
        gameObject.transform.position = position;
        var circle = new SanctifierCircle();
        circle.Renderer = gameObject.GetComponent<SpriteRenderer>();
        circle.Renderer.enabled = PlayerControl.LocalPlayer.IsRole<SanctifierRole>();
        circle._scale = scale;
        circle._shown = PlayerControl.LocalPlayer.IsRole<SanctifierRole>() || OptionGroupSingleton<SanctifierOptions>.Instance.ShowSanctify;
        circle.GameObject = gameObject;
        SanctifierCircles.Add(circle);
        circle.Start();
        return circle;
    }
    public static void Clear()
    {
        foreach(var circle in SanctifierCircles)
        {
            circle.GameObject.Destroy();
        }
        SanctifierCircles.Clear();
    }
    public void Destroy()
    {
        SanctifierCircles.Remove(this);
        GameObject?.Destroy();
    }
    public static bool IsInCircle(Transform transform)
    {
        var result = false;
        foreach (var circle in SanctifierCircles)
        {
            if (circle?.GameObject?.transform != null && Vector2.Distance(circle.GameObject.transform.position, transform.position) <= circle._scale)
            {
                result = true;
            }
        }
        return result;
    }
}
[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class UpdateSanctifierCircles
{
    public static void Postfix(HudManager __instance)
    {
        foreach (var circle in SanctifierCircle.SanctifierCircles)
        {
            circle.Update();
        }
    }
}
