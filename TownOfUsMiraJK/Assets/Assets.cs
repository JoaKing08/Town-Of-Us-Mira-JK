using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Assets;

public static class ToUJKAssets
{
    private const string ShortPath = "TownOfUsMiraJK.Resources";
    public static readonly AssetBundle MainBundle = AssetBundleManager.Load("toumjk-assets");
    public static readonly AssetBundle SoundVBundle = AssetBundleManager.Load("soundvision");
    public static LoadableAsset<Sprite> Banner { get; } = new LoadableResourceAsset($"{ShortPath}.Banner.png");
    public static LoadableAsset<GameObject> SanctifierCircle { get; }
        = new LoadableBundleAsset<GameObject>("SanctifierCircle", MainBundle);
    public static LoadableAsset<GameObject> GuardLine { get; }
        = new LoadableBundleAsset<GameObject>("GuardLine", MainBundle);
    public static LoadableAsset<Material> SoundV { get; }
        = new LoadableBundleAsset<Material>("SoundV", SoundVBundle);
    public static LoadableAsset<Sprite> ApocBubble { get; } = new LoadableResourceAsset($"{ShortPath}.ChatApocBubble.png");
}
