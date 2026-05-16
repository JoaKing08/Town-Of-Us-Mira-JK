using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Assets;

public static class ModifierIcons
{
    private const string ShortPath = "TownOfUsMiraJK.Resources";
    
    // Universal
    public static LoadableAsset<Sprite> Drunk { get; } = new LoadableResourceAsset($"{ShortPath}.ModifierIcons.Drunk.png", 200);

    // Impostor
    public static LoadableAsset<Sprite> Tasker { get; } = new LoadableResourceAsset($"{ShortPath}.ModifierIcons.Tasker.png", 200);

    // Alliance
    public static LoadableAsset<Sprite> Prophet { get; } = new LoadableResourceAsset($"{ShortPath}.ModifierIcons.Prophet.png", 200);
}