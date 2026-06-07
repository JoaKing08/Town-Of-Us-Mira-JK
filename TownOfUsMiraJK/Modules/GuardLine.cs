using Il2CppInterop.Runtime.Injection;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using TownOfUsMiraJK.Assets;
using UnityEngine;

public static class GuardLineManager
{
    public static GuardLine Create(Transform parent = null, int points = 360, Color? color = null, float radius = 1f, float rotationSpeed = 1f)
    {
        var gameObject = GameObject.Instantiate(ToUJKAssets.GuardLine.LoadAsset());
        if (parent != null)
        {
            gameObject.transform.SetParent(parent);
        }
        var line = gameObject.AddComponent<GuardLine>();
        line.Points = points;
        line.Color = color ?? Color.white;
        line.Radius = radius;
        line.RotationSpeed = rotationSpeed;
        return line;
    }
}

[RegisterInIl2Cpp]
public class GuardLine(IntPtr ptr) : MonoBehaviour(ptr)
{
    public int Points { get; set; } = 360;
    public Color Color { get; set; } = Color.white;
    public float Radius { get; set; } = 1f;
    public float RotationSpeed { get; set; } = 1f;
    public LineRenderer Renderer => gameObject.GetComponent<LineRenderer>();
    private float _timer;
    public void UpdateGuardLine(float angleOffset = 0)
    {
        Renderer.positionCount = Points;
        Renderer.SetColors(Color, Color);
        for (float i = 0; i < Renderer.positionCount; i++)
        {
            var angle = i + angleOffset;
            var position = gameObject.transform.position;
            position.x += Mathf.Cos(angle * Mathf.PI * 2f / Renderer.positionCount) * Radius;
            position.y += Mathf.Sin(angle * Mathf.PI * 2f / Renderer.positionCount) * Radius;
            Renderer.SetPosition((int)i, position);
        }
    }
    public void Start()
    {
        UpdateGuardLine();
    }
    public void Update()
    {
        _timer += Time.deltaTime;
        UpdateGuardLine(_timer * RotationSpeed);
    }
    public void Destroy()
    {
        gameObject.Destroy();
    }
}
