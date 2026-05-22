using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Assets;

public static class SecrAssets
{
    private const string ShortPath = "TownOfUsMiraJK.Resources.SecrButtons";
    public static LoadableAsset<Sprite> AmmitVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.AmmitVentSprite.png");
    public static LoadableAsset<Sprite> AmmitDevourSprite { get; } = new LoadableResourceAsset($"{ShortPath}.AmmitDevourSprite.png");
    public static LoadableAsset<Sprite> ShadowVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowVentSprite.png");
    public static LoadableAsset<Sprite> ShadowKillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowKillSprite.png");
    public static LoadableAsset<Sprite> ShadowVanishSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowVanishSprite.png");
    public static LoadableAsset<Sprite> ShadowAppearSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowAppearSprite.png");
    public static LoadableAsset<Sprite> ShadowDarknessSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowDarknessSprite.png");
}