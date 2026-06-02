using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using System.Text;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class InspectorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public int Seed { get; set; } = 0;
    public DoomableType DoomHintType => DoomableType.Insight;
    public string LocaleKey => "Inspector";
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
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Inspect", "Inspect"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}InspectWikiDescription"),
                    CrewAssets.InspectorInspectSprite)
            };
        }
    }

    public Color RoleColor => Colors.Inspector;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = RoleIcons.Inspector,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        IntroSound = TouAudio.DetectiveIntroSound
    };
    public void ReportOnInspect()
    {
        if (!Player.AmOwner)
        {
            return;
        }

        var inspected = ModifierUtils
            .GetPlayersWithModifier<InspectorInspectModifier>([HideFromIl2Cpp] (x) => x.Inspector == Player && !x.Player.HasDied()).FirstOrDefault();

        if (inspected == null)
        {
            return;
        }

        var report = BuildReport(inspected);

        var title = $"<color=#{Colors.Inspector.ToHtmlStringRGBA()}>{TouLocale.GetParsed("TouJKRoleInspectorInspectTitle")}</color>";
        MiscUtils.AddFakeChat(inspected.Data, title, report, false, true);
    }
    public string BuildReport(PlayerControl player)
    {
        if (OptionGroupSingleton<InspectorOptions>.Instance.UseDoomResults)
        {
            var reportBuilder = new StringBuilder();
            var role = player.Data.Role;
            var doomableRole = role as IDoomable;
            var undoomableRole = role as IUnguessable;
            var hintType = DoomableType.Default;
            var cachedMod =
                player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) as ICachedRole;
            if (cachedMod != null)
            {
                role = cachedMod.CachedRole;
                doomableRole = role as IDoomable;
            }

            if (undoomableRole != null)
            {
                role = undoomableRole.AppearAs;
                doomableRole = role as IDoomable;
            }

            if (doomableRole != null)
            {
                hintType = doomableRole.DoomHintType;
            }

            var fallback = TouLocale.GetParsed("TouJKRoleInspectorRoleHintDefault");
            var hint = TouLocale.GetParsed($"TouJKRoleInspectorRoleHint{hintType}");

            if (hint.Contains("STRMISS"))
            {
                reportBuilder.AppendLine(TownOfUsPlugin.Culture,
                    $"{fallback.Replace("<player>", player.Data.PlayerName)}\n");
            }
            else
            {
                reportBuilder.AppendLine(TownOfUsPlugin.Culture, $"{hint.Replace("<player>", player.Data.PlayerName)}\n");
            }

            var roles = MiscUtils.AllRegisteredRoles
                .Where(x => (x is IDoomable doomRole && doomRole.DoomHintType == DoomableType.Default &&
                    x is not IUnguessable || x is not IDoomable) && !x.IsDead).ToList();
            roles = roles.OrderBy(x => x.GetRoleName()).ToList();
            var lastRole = roles[roles.Count - 1];

            if (hintType != DoomableType.Default)
            {
                roles = MiscUtils.AllRoles
                    .Where(x => x is IDoomable doomRole && doomRole.DoomHintType == hintType && x is not IUnguessable)
                    .OrderBy(x => x.GetRoleName()).ToList();
                lastRole = roles[roles.Count - 1];
            }

            if (roles.Count != 0)
            {
                reportBuilder.Append(TownOfUsPlugin.Culture, $"(");
                foreach (var role2 in roles)
                {
                    if (role2 == lastRole)
                    {
                        reportBuilder.Append(TownOfUsPlugin.Culture,
                            $"{MiscUtils.GetHyperlinkText(lastRole)})");
                    }
                    else
                    {
                        reportBuilder.Append(TownOfUsPlugin.Culture,
                            $"{MiscUtils.GetHyperlinkText(role2)}, ");
                    }
                }
            }
            return reportBuilder.ToString();
        }
        else
        {
            if (Seed == 0)
            {
                while (Seed == 0)
                {
                    Seed = UnityEngine.Random.RandomRangeInt(int.MinValue, int.MaxValue);
                }
            }
            int playerSeed = Seed + player.PlayerId;
            RoleBehaviour role = player.Data.Role;
            var cachedMod =
                player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) as ICachedRole;
            if (cachedMod != null)
            {
                role = cachedMod.CachedRole;
            }
            var unguessable = player.Data.Role as IUnguessable;
            if (unguessable != null)
            {
                role = unguessable.AppearAs;
            }
            int crewCount = (int)OptionGroupSingleton<InspectorOptions>.Instance.CrewRoles;
            int neutCount = (int)OptionGroupSingleton<InspectorOptions>.Instance.NeutRoles;
            int impCount = (int)OptionGroupSingleton<InspectorOptions>.Instance.ImpRoles;

            List<RoleBehaviour> crewRoles = MiscUtils.AllRegisteredRoles.Where(x => x.IsCrewmate() && CustomRoleUtils.CanSpawnOnCurrentMode(x) && GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(x.Role) > 0 && GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(x.Role) > 0 && !x.IsDead).ToList();
            List<RoleBehaviour> neutRoles = MiscUtils.AllRegisteredRoles.Where(x => !x.IsCrewmate() && !x.IsImpostor() && CustomRoleUtils.CanSpawnOnCurrentMode(x) && GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(x.Role) > 0 && GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(x.Role) > 0 && !x.IsDead).ToList();
            List<RoleBehaviour> impRoles = MiscUtils.AllRegisteredRoles.Where(x => x.IsImpostor() && CustomRoleUtils.CanSpawnOnCurrentMode(x) && GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(x.Role) > 0 && GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(x.Role) > 0 && !x.IsDead).ToList();
            crewRoles.Add(MiscUtils.GetRegisteredRole(AmongUs.GameOptions.RoleTypes.Crewmate));
            impRoles.Add(MiscUtils.GetRegisteredRole(AmongUs.GameOptions.RoleTypes.Impostor));
            if (GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetNumPerGame(Role) <= 1)
            {
                crewRoles = crewRoles.Where(x => x.Role != Role).ToList();
            }
            if (player.IsCrewmate())
            {
                crewCount -= 1;
                crewRoles.Remove(role);
            }
            else if (player.IsImpostor())
            {
                impCount -= 1;
                impRoles.Remove(role);
            }
            else
            {
                neutCount -= 1;
                neutRoles.Remove(role);
            }
            var random = new System.Random(playerSeed);
            crewRoles.Shuffle(ref random);
            neutRoles.Shuffle(ref random);
            impRoles.Shuffle(ref random);
            List<RoleBehaviour> showedRoles = new();
            showedRoles.Add(role);
            if (crewRoles.Count != 0)
            {
                var count = Math.Min(crewCount, crewRoles.Count);
                showedRoles.AddRange(crewRoles.GetRange(0, crewCount));
            }
            if (neutRoles.Count != 0)
            {
                var count = Math.Min(neutCount, neutRoles.Count);
                showedRoles.AddRange(neutRoles.GetRange(0, neutCount));
            }
            if (impRoles.Count != 0)
            {
                var count = Math.Min(impCount, impRoles.Count);
                showedRoles.AddRange(impRoles.GetRange(0, impCount));
            }
            showedRoles = showedRoles.OrderBy(x => x.GetRoleName()).ToList();
            var reportBuilder = new StringBuilder();
            reportBuilder.AppendLine(TownOfUsPlugin.Culture, $"{TouLocale.GetParsed("TouJKRoleInspectorRoleListing").Replace("<player>", player.Data.PlayerName)}: ");
            foreach (var showedRole in showedRoles)
            {
                reportBuilder.Append(TownOfUsPlugin.Culture, $"{MiscUtils.GetHyperlinkText(showedRole)}, ");
            }
            reportBuilder.Remove(reportBuilder.Length - 2, 2);
            return reportBuilder.ToString();
        }
    }
}