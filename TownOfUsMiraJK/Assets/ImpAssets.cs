using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Assets;

public static class ToUJKImpAssets
{
    private const string ShortPath = "TownOfUsMiraJK.Resources.ImpButtons";
    public static LoadableAsset<Sprite> PoisonerPoisonSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PoisonerPoisonSprite.png");
    public static LoadableAsset<Sprite> SniperAimSprite { get; } = new LoadableResourceAsset($"{ShortPath}.SniperAimSprite.png");
    public static LoadableAsset<Sprite> SniperShootSprite { get; } = new LoadableResourceAsset($"{ShortPath}.SniperShootSprite.png");
    public static LoadableAsset<Sprite> GodfatherRecruitSprite { get; } = new LoadableResourceAsset($"{ShortPath}.GodfatherRecruitSprite.png");
    public static LoadableAsset<Sprite> PsychopathInsanitySprite { get; } = new LoadableResourceAsset($"{ShortPath}.PsychopathInsanitySprite.png");
}