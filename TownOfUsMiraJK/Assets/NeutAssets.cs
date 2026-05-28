using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUsMiraJK.Assets;

public static class NeutAssets
{
    private const string ShortPath = "TownOfUsMiraJK.Resources.NeutButtons";
    public static LoadableAsset<Sprite> FamineVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.FamineVentSprite.png");
    public static LoadableAsset<Sprite> FamineStarveSprite { get; } = new LoadableResourceAsset($"{ShortPath}.FamineStarveSprite.png");
    public static LoadableAsset<Sprite> BakerBreadSprite { get; } = new LoadableResourceAsset($"{ShortPath}.BakerBreadSprite.png");
    public static LoadableAsset<Sprite> BerserkerKillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.BerserkerKillSprite.png");
    public static LoadableAsset<Sprite> BerserkerVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.BerserkerVentSprite.png");
    public static LoadableAsset<Sprite> WarKillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.WarKillSprite.png");
    public static LoadableAsset<Sprite> WarVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.WarVentSprite.png");
    public static LoadableAsset<Sprite> ReaperReapSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ReaperReapSprite.png");
    public static LoadableAsset<Sprite> PirateDuelSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PirateDuelSprite.png");
    public static LoadableAsset<Sprite> PirateScimitarSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PirateScimitarSprite.png", 150);
    public static LoadableAsset<Sprite> PirateRapierSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PirateRapierSprite.png", 150);
    public static LoadableAsset<Sprite> PiratePistolSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PiratePistolSprite.png", 150);
    public static LoadableAsset<Sprite> PirateSidestepSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PirateSidestepSprite.png", 150);
    public static LoadableAsset<Sprite> PirateChainmailSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PirateChainmailSprite.png", 150);
    public static LoadableAsset<Sprite> PirateBackpedalSprite { get; } = new LoadableResourceAsset($"{ShortPath}.PirateBackpedalSprite.png", 150);
    public static LoadableAsset<Sprite> BloodhoundKillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.BloodhoundKillSprite.png");
    public static LoadableAsset<Sprite> BloodhoundVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.BloodhoundVentSprite.png");
    public static LoadableAsset<Sprite> WitchMarkSprite { get; } = new LoadableResourceAsset($"{ShortPath}.WitchMarkSprite.png");
    public static LoadableAsset<Sprite> WitchControlSprite { get; } = new LoadableResourceAsset($"{ShortPath}.WitchControlSprite.png");
    public static LoadableAsset<Sprite> CursedSoulSoulSwapSprite { get; } = new LoadableResourceAsset($"{ShortPath}.CursedSoulSoulSwapSprite.png");
    public static LoadableAsset<Sprite> NecromancerReanimateSprite { get; } = new LoadableResourceAsset($"{ShortPath}.NecromancerReanimateSprite.png");
    public static LoadableAsset<Sprite> JackalKillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.JackalKillSprite.png");
    public static LoadableAsset<Sprite> JackalVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.JackalVentSprite.png");
    public static LoadableAsset<Sprite> AmmitVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.AmmitVentSprite.png");
    public static LoadableAsset<Sprite> AmmitDevourSprite { get; } = new LoadableResourceAsset($"{ShortPath}.AmmitDevourSprite.png");
    public static LoadableAsset<Sprite> ShadowVentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowVentSprite.png");
    public static LoadableAsset<Sprite> ShadowKillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowKillSprite.png");
    public static LoadableAsset<Sprite> ShadowVanishSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowVanishSprite.png");
    public static LoadableAsset<Sprite> ShadowAppearSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowAppearSprite.png");
    public static LoadableAsset<Sprite> ShadowDarknessSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ShadowDarknessSprite.png");
    public static LoadableAsset<Sprite> ManhunterKillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ManhunterKillSprite.png");
}