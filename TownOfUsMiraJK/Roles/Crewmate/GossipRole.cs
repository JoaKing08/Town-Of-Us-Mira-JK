using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities.Extensions;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class GossipRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public DoomableType DoomHintType => DoomableType.Insight;
    public string LocaleKey => "Gossip";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Chat", "Chat"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}ChatWikiDescription"),
                    CrewAssets.GossipChatSprite)
            };
        }
    }

    public Color RoleColor => Colors.Gossip;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = RoleIcons.Gossip,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        IntroSound = TouAudio.PoliticianIntroSound
    };
    public void ReportOnChat()
    {
        if (!Player.AmOwner)
        {
            return;
        }

        var chated = ModifierUtils
            .GetPlayersWithModifier<GossipChatModifier>([HideFromIl2Cpp] (x) => x.Gossip == Player && !x.Player.HasDied()).FirstOrDefault();

        if (chated == null)
        {
            return;
        }

        var report = BuildReport(chated);

        var title = $"<color=#{Colors.Gossip.ToHtmlStringRGBA()}>{TouLocale.GetParsed("TouJKRoleGossipChatTitle")}</color>";
        MiscUtils.AddFakeChat(chated.Data, title, report, false, true);
    }
    public string BuildReport(PlayerControl player)
    {
        var players = new List<PlayerControl>() { player };
        var possiblePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.AmOwner && x.PlayerId != player.PlayerId && (!x.HasDied() || OptionGroupSingleton<GossipOptions>.Instance.UseDead)).ToList();
        for (int i = 0; i < OptionGroupSingleton<GossipOptions>.Instance.Players - 1; i++)
        {
            var toAdd = possiblePlayers.Random();
            if (toAdd == null)
            {
                return TouLocale.GetParsed("TouJKRoleGossipChatResultNotEnoughPlayers");
            }
            else
            {
                players.Add(toAdd);
                possiblePlayers.Remove(toAdd);
            }
        }
        var role = players.Random()!.Data.Role;
        var lastPlayer = players[-1];
        players.Remove(lastPlayer);
        string playerString = new string(players.SelectMany(x => x.Data.PlayerName + ", ").ToArray());
        playerString = playerString.Remove(playerString.Length - 2);
        playerString += TouLocale.GetParsed("TouJKRoleGossipChatResultOr") + lastPlayer.Data.PlayerName;
        return TouLocale.GetParsed("TouJKRoleGossipChatResult").Replace("<players>", playerString).Replace("role", MiscUtils.GetHyperlinkText(role));
    }
}