using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Modules;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Roles.Crewmate;

public sealed class PsychicRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public override bool IsAffectedByComms => false;
    public DoomableType DoomHintType => DoomableType.Perception;
    public string LocaleKey => "Psychic";
    public string RoleName => TouLocale.Get($"TouJKRole{LocaleKey}");
    public string RoleDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}IntroBlurb");
    public string RoleLongDescription => TouLocale.GetParsed($"TouJKRole{LocaleKey}TabDescription");

    public string GetAdvancedDescription()
    {
        return
            TouLocale.GetParsed($"TouJKRole{LocaleKey}WikiDescription") +
            MiscUtils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            CameraEffect.Initialize();
            CameraEffect.singleton.materials.Clear();
            CameraEffect.singleton.materials.Add(ToUJKAssets.SoundV.LoadAsset());
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        if (Player.AmOwner)
        {
            CameraEffect.singleton.materials.Clear();
        }
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
                new(TouLocale.GetParsed($"TouJKRole{LocaleKey}Guard", "Guard"),
                    TouLocale.GetParsed($"TouJKRole{LocaleKey}GuardWikiDescription"),
                    CrewAssets.BodyguardGuardSprite)
            };
        }
    }

    public Color RoleColor => Colors.Psychic;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.MediumIntroSound,
        OptionsScreenshot = TouBanners.CrewmateRoleBanner,
        Icon = RoleIcons.Psychic
    };

    public static DateTime RoundStart = DateTime.UtcNow;
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class UpdatePlayerVisuals
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (PlayerControl.LocalPlayer?.IsRole<PsychicRole>() != true || __instance.AmOwner || __instance.HasDied())
            {
                return;
            }
            var color = Color.white;
            var roleName = TouLocale.Get("TouJKRolePsychicUnknown");
            if ((DateTime.UtcNow - RoundStart).TotalSeconds < OptionGroupSingleton<PsychicOptions>.Instance.Invis)
            {
                color = Color.clear;
                roleName = "";
            }
            else if (__instance.TryGetModifier<PsychicColoredModifier>(out var modifier) && modifier.Charges >= OptionGroupSingleton<PsychicOptions>.Instance.MindscanCount)
            {
                switch (OptionGroupSingleton<PsychicOptions>.Instance.MindscanVisibility)
                {
                    case PsychicSees.Faction:
                        if (__instance.IsCrewmate())
                        {
                            color = Color.cyan;
                            roleName = TouLocale.Get("CrewmateKeyword");
                        }
                        else if (__instance.IsImpostor())
                        {
                            color = Color.red;
                            roleName = TouLocale.Get("ImpostorKeyword");
                        }
                        else
                        {
                            color = Color.gray;
                            roleName = TouLocale.Get("NeutralKeyword");
                        }
                        break;
                    case PsychicSees.Alignment:
                        if (__instance.IsCrewmate())
                        {
                            color = Color.cyan;
                        }
                        else if (__instance.IsImpostor())
                        {
                            color = Color.red;
                        }
                        else
                        {
                            color = Color.gray;
                        }
                        roleName = MiscUtils.GetParsedRoleAlignment(__instance.Data.Role);
                        break;
                    case PsychicSees.Role:
                        color = __instance.Data.Role.TeamColor;
                        roleName = __instance.Data.Role.GetRoleName();
                        break;
                }
            }
            ColorCharacter(__instance, color, roleName: roleName);
        }
        public static void ColorCharacter(PlayerControl player, Color color, byte? colorId = null, string? roleName = null)
        {
            if (colorId == null)
            {
                var closestColor = Palette.PlayerColors.Where(x => x.r != 0 || x.g != 0 || x.b != 0).MinBy(x =>
                Mathf.Abs((color.r * byte.MaxValue) - x.r) + Mathf.Abs((color.g * byte.MaxValue) - x.g) + Mathf.Abs((color.b * byte.MaxValue) - x.b));
                colorId = (byte?)Palette.PlayerColors.IndexOf(closestColor);
            }
            if (player.TryGetModifier<PsychicColoredModifier>(out var modifier))
            {
                modifier.UpdateData(color, colorId, roleName);
            }
            else
            {
                player.AddModifier<PsychicColoredModifier>(PlayerControl.LocalPlayer, color, colorId ?? 0, roleName ?? "");
            }
        }
    }
}