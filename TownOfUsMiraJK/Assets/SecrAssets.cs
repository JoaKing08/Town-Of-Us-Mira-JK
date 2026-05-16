using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Assets;

public static class SecrAssets
{
    private const string ShortPath = "TownOfUsMiraJK.Resources.SecrButtons";
    public static LoadableAsset<Sprite> AmmitVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.AmmitVentSprite.png");
    public static LoadableAsset<Sprite> AmmitDevourSprite { get; } = new LoadableResourceAsset($"{ShortPath}.AmmitDevourSprite.png");
}