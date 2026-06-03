using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Assets;

public static class CrewAssets
{
    private const string ShortPath = "TownOfUsMiraJK.Resources.CrewButtons";
    public static LoadableAsset<Sprite> MonsterHunterStakeSprite { get; } = new LoadableResourceAsset($"{ShortPath}.MonsterHunterStakeSprite.png");
    public static LoadableAsset<Sprite> InspectorInspectSprite { get; } = new LoadableResourceAsset($"{ShortPath}.InspectorInspectSprite.png");
    public static LoadableAsset<Sprite> TavernKeeperDrinkSprite { get; } = new LoadableResourceAsset($"{ShortPath}.TavernKeeperDrinkSprite.png");
    public static LoadableAsset<Sprite> WatcherWatchSprite { get; } = new LoadableResourceAsset($"{ShortPath}.WatcherWatchSprite.png");
    public static LoadableAsset<Sprite> BodyguardGuardSprite { get; } = new LoadableResourceAsset($"{ShortPath}.BodyguardGuardSprite.png");
    public static LoadableAsset<Sprite> CrusaderFortifySprite { get; } = new LoadableResourceAsset($"{ShortPath}.CrusaderFortifySprite.png");
    public static LoadableAsset<Sprite> GunslingerAimSprite { get; } = new LoadableResourceAsset($"{ShortPath}.GunslingerAimSprite.png");
    public static LoadableAsset<Sprite> GunslingerShootSprite { get; } = new LoadableResourceAsset($"{ShortPath}.GunslingerShootSprite.png", 150);
    public static LoadableAsset<Sprite> CoronerAutopsySprite { get; } = new LoadableResourceAsset($"{ShortPath}.CoronerAutopsySprite.png");
    public static LoadableAsset<Sprite> SanctifierSanctifySprite { get; } = new LoadableResourceAsset($"{ShortPath}.SanctifierSanctifySprite.png");
    public static LoadableAsset<Sprite> PsychicMindscanSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PsychicMindscanSprite.png");
    public static LoadableAsset<Sprite> GossipChatSprite { get; } = new LoadableResourceAsset($"{ShortPath}.GossipChatSprite.png");
}