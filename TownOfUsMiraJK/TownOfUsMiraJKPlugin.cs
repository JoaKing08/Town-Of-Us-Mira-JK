using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using DivaniMods;
using HarmonyLib;
using MiraAPI;
using MiraAPI.Modifiers;
using MiraAPI.PluginLoading;
using NotePadMod;
using PerfectComms.Api;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using System.Globalization;
using System.Reflection;
using TouMiraRolesExtension;
using TownOfExtra;
using TownOfUs;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Crewmate;
using UnityEngine;
using VoiceChatPlugin.VoiceChat;
using static Rewired.Platforms.Custom.CustomPlatformUnifiedKeyboardSource.KeyPropertyMap;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace TownOfUsMiraJK;

[BepInAutoPlugin("joaking.tou.joakingaddon", "Town Of Us Mira JoaKing's Addon")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[BepInDependency(TownOfUsPlugin.Id)]
[BepInDependency(DivaniId, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(ExtensionId, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(ExtraId, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(CommsId, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(NotePadId, BepInDependency.DependencyFlags.SoftDependency)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class TownOfUsMiraJKPlugin : BasePlugin, IMiraPlugin
{
    private const string DivaniId = "com.divani.mods";
    private const string ExtensionId = "toumiragames.tou.extension";
    private const string ExtraId = "me.mehzxzz.townOfExtra";
    private const string CommsId = "com.edgetel.perfectcomms";
    private const string NotePadId = "maxi.notepad";
    public static bool DivaniInstalled => IL2CPPChainloader.Instance.Plugins.ContainsKey(DivaniId);
    public static bool ExtensionInstalled => IL2CPPChainloader.Instance.Plugins.ContainsKey(ExtensionId);
    public static bool ExtraInstalled => IL2CPPChainloader.Instance.Plugins.ContainsKey(ExtraId);
    public static bool CommsInstalled => IL2CPPChainloader.Instance.Plugins.ContainsKey(CommsId);
    public static bool NotePadInstalled => IL2CPPChainloader.Instance.Plugins.ContainsKey(NotePadId);

    public static CultureInfo Culture => TownOfUs.TownOfUsPlugin.Culture;

    public string OptionsTitleText => "TOU JoaKing";

    public static bool IsDevBuild => true;

    public ConfigFile GetConfigFile()
    {
        return Config;
    }

    public Harmony Harmony { get; } = new(Id);

    public override void Load()
    {
        ReactorCredits.Register("Town Of Us Mira JoaKing's Addon", Version, IsDevBuild, ReactorCredits.AlwaysShow);
        IL2CPPChainloader.Instance.Finished += Modules.ExtensionLocale.SearchInternalLocale;

        Harmony.PatchAll(Assembly.GetExecutingAssembly());
        
        if (CommsInstalled)
        {
            PerfectCommsApi.RegisterVoiceRule(Id, ctx =>
            {
                if (!ctx.IsLocal && !PlayerControl.LocalPlayer.HasDied() && ctx.Player.TryGetModifier<AmmitDevouredModifier>(out var modifier) && !PlayerControl.LocalPlayer.HasModifier<AmmitDevouredModifier>(x => x.Ammit == modifier.Ammit) && !modifier.Ammit.AmOwner)
                {
                    return VoiceRuleResult.Mute("Devoured");
                }
                else if (!ctx.IsLocal && !PlayerControl.LocalPlayer.HasDied() && ctx.Phase == VoicePhaseKind.Tasks && PlayerControl.LocalPlayer.IsRole<PsychicRole>())
                {
                    return VoiceRuleResult.Muffle("Colorless");
                }

                return VoiceRuleResult.Pass;
            });
            PerfectCommsApi.RegisterVoiceChannel(Id, ctx =>
            {
                if (!PlayerControl.LocalPlayer.HasDied() && !ctx.Player.HasDied() && PlayerControl.LocalPlayer.IsRole<UndercoverRole>() && ctx.Player.IsImpostorAligned() && VoiceChatGameOptions.Instance.MaxChatDistance.Value < Distance(PlayerControl.LocalPlayer.transform.position, ctx.Player.transform.position))
                {
                    return new VoiceChannelResult("undercover_radio:undercover", Shape: VoiceAudioShape.Muffle);
                }
                else if (!PlayerControl.LocalPlayer.HasDied() && !ctx.Player.HasDied() && PlayerControl.LocalPlayer.IsImpostorAligned() && ctx.Player.IsRole<UndercoverRole>())
                {
                    return new VoiceChannelResult("undercover_radio:impostor");
                }
                return null;
            });
        }
    }
    private static float Distance(Vector2 a, Vector2 b)
    {
        float dx = a.x - b.x;
        float dy = a.y - b.y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
}
