using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Assets;

public static class ToUJKModifierIcons
{
    private const string ShortPath = "TownOfUsMiraJK.Resources";

    // Crewmate
    public static LoadableAsset<Sprite> Explorer { get; } = new LoadableResourceAsset($"{ShortPath}.ModifierIcons.Explorer.png", 200);

    // Universal
    public static LoadableAsset<Sprite> Drunk { get; } = new LoadableResourceAsset($"{ShortPath}.ModifierIcons.Drunk.png", 200);

    // Impostor
    public static LoadableAsset<Sprite> Tasker { get; } = new LoadableResourceAsset($"{ShortPath}.ModifierIcons.Tasker.png", 200);
    public static LoadableAsset<Sprite> Outcast { get; } = new LoadableResourceAsset($"{ShortPath}.ModifierIcons.Outcast.png", 200);

    // Alliance
    public static LoadableAsset<Sprite> Prophet { get; } = new LoadableResourceAsset($"{ShortPath}.ModifierIcons.Prophet.png", 200);
    public static LoadableAsset<Sprite> Rivals { get; } = new LoadableResourceAsset($"{ShortPath}.ModifierIcons.Rivals.png", 200);
}