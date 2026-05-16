using MiraAPI.Utilities;
using TownOfUs;
using UnityEngine;

namespace TownOfUsMiraJK;

public static class Colors
{
    // Crew Colors
    public static Color MonsterHunter => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(178, 178, 230, 255);
    public static Color Inspector => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(127, 255, 219, 255);
    public static Color TavernKeeper => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(140, 69, 20, 255);
    public static Color Undercover => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(0, 38, 0, 255);
    public static Color Watcher => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(128, 223, 223, 255);
    public static Color Bodyguard => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(54, 69, 79, 255);
    public static Color Crusader => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(239, 239, 239, 255);
    public static Color Executor => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(170, 170, 170, 255);
    public static Color Coroner => TownOfUsColors.UseBasic ? Palette.CrewmateBlue : new Color32(180, 160, 160, 255);

    // Neutral Colors
    public static Color Baker => new Color32(255, 191, 128, 255);
    public static Color Berserker => new Color32(255, 79, 0, 255);
    public static Color SoulCollector => new Color32(224, 0, 255, 255);
    public static Color Famine => new Color32(97, 128, 97, 255);
    public static Color War => new Color32(128, 31, 0, 255);
    public static Color Death => new Color32(76, 0, 76, 255);
    public static Color Pirate => new Color32(235, 193, 62, 255);
    public static Color Bloodhound => new Color32(251, 29, 76, 255);
    public static Color Witch => new Color32(191, 96, 255, 255);
    public static Color CursedSoul => new Color32(128, 0, 255, 255);
    public static Color Necromancer => new Color32(103, 149, 86, 255);
    public static Color Jackal => new Color32(102, 102, 102, 255);
    public static Color Harbinger => new Color32(51, 76, 102, 255);

    // Modifiers
    public static Color Drunk => new Color32(117, 128, 0, 255);

    // Secret

    //   Crew
    public static Color Sanctifier => new Color32(255, 255, 192, 255);

    //   Neutral
    public static Color Shadow => new Color32(48, 32, 48, 255);
    public static Color Ammit => new Color32(85, 107, 47, 255);
    public static Color Manhunter => new Color32(204, 102, 0, 255);

    //   Impostor
    public static Color Outcast => Palette.ImpostorRed;

    // Other Colors
    public static Color Apocalypse => new Color32(96, 96, 96, 255);
}