using Il2CppInterop.Runtime.Injection;
using MiraAPI.GameOptions;
using Reactor.Utilities.Extensions;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;

public class SanctifierCircle : MonoBehaviour
{
    static SanctifierCircle() => ClassInjector.RegisterTypeInIl2Cpp<SanctifierCircle>();
    public SanctifierCircle(IntPtr ptr) : base(ptr) { }
    public static List<SanctifierCircle> SanctifierCircles => GameObject.FindObjectsOfType<SanctifierCircle>().ToList();
    public float RotationSpeed { get; set; } = -10f;
    public float MinimumAlpha { get; set; } = 0f;
    public float MaximumAlpha { get; set; } = 1f;
    public float CycleDuration { get; set; } = 10f;
    public SpriteRenderer Renderer => gameObject.GetComponent<SpriteRenderer>();
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
        if (!_shown)
        {
            _showTimer += Time.deltaTime;
            if (_showTimer >= OptionGroupSingleton<SanctifierOptions>.Instance.ShowSanctifyDelay.Value)
            {
                Renderer.enabled = true;
                _shown = true;
            }
        }
        gameObject.transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);
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
        var circle = gameObject.AddComponent<SanctifierCircle>();
        circle.Renderer.enabled = PlayerControl.LocalPlayer.IsRole<SanctifierRole>();
        circle._scale = scale;
        circle._shown = PlayerControl.LocalPlayer.IsRole<SanctifierRole>() || OptionGroupSingleton<SanctifierOptions>.Instance.ShowSanctify;
        return circle;
    }
    public static void Clear()
    {
        foreach(var circle in SanctifierCircles)
        {
            circle.gameObject.Destroy();
        }
    }
    public void Destroy()
    {
        gameObject.Destroy();
    }
    public static bool IsInCircle(Transform transform)
    {
        var result = false;
        foreach (var circle in SanctifierCircles)
        {
            if (circle?.gameObject?.transform != null && Vector2.Distance(circle.gameObject.transform.position, transform.position) <= circle._scale / 2)
            {
                result = true;
            }
        }
        return result;
    }
}
