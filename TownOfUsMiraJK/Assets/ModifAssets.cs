using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Assets;

public static class ToUJKModifAssets
{
    private const string ShortPath = "TownOfUsMiraJK.Resources.ModifButtons";
    public static LoadableAsset<Sprite> ExplorerVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ExplorerVentSprite.png");
}