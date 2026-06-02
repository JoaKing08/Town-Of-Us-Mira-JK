using TownOfUs.Modifiers.Crewmate.Coroner;
using TownOfUs.Modules.Localization;

namespace TownOfUsMiraJK.Modifiers.Coroner
{
    public sealed class CoronerKillerColor(DeadBody body) : CoronerResultBaseModifier(body)
    {
        public override string MeetingMessage()
        {
            switch (CoronerData?.KillerColor)
            {
                case "lighter":
                    return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerColorLight");
                case "darker":
                    return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerColorDark");
                default:
                    return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerColorUnknown");
            }
        }
        public override string NotificationMessage()
        {
            switch (CoronerData?.KillerColor)
            {
                case "lighter":
                    return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerColorLight");
                case "darker":
                    return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerColorDark");
                default:
                    return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerColorUnknown");
            }
        }
    }
}
