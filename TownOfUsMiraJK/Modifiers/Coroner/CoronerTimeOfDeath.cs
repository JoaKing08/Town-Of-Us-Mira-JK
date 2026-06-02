using TownOfUs.Modifiers.Crewmate.Coroner;
using TownOfUs.Modules.Localization;

namespace TownOfUsMiraJK.Modifiers.Coroner
{
    public sealed class CoronerTimeOfDeath(DeadBody body) : CoronerResultBaseModifier(body)
    {
        public override string MeetingMessage()
        {
            return TouLocale.GetParsed("TouJKRoleCoronerAutopsyTimeOfDeath").Replace("<time>", $"{((int?)CoronerData?.KilledAgo ?? 0) / 1000}");
        }
        public override string NotificationMessage()
        {
            return TouLocale.GetParsed("TouJKRoleCoronerAutopsyTimeOfDeath").Replace("<time>", $"{((int?)CoronerData?.KilledAgo ?? 0) / 1000}");
        }
    }
}
