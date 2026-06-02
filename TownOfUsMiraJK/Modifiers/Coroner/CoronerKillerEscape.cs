using TownOfUs.Modifiers.Crewmate.Coroner;
using TownOfUs.Modules.Localization;

namespace TownOfUsMiraJK.Modifiers.Coroner
{
    public sealed class CoronerKillerEscape(DeadBody body) : CoronerResultBaseModifier(body)
    {
        public override string MeetingMessage()
        {
            return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscape{CoronerData?.KillerEscaped ?? Enums.KillerEscape.Nowhere}");
        }
        public override string NotificationMessage()
        {
            return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscape{CoronerData?.KillerEscaped ?? Enums.KillerEscape.Nowhere}");
        }
    }
}
