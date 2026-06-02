using TownOfUs.Modifiers.Crewmate.Coroner;
using TownOfUs.Modules.Localization;

namespace TownOfUsMiraJK.Modifiers.Coroner
{
    public sealed class CoronerKillerKills(DeadBody body) : CoronerResultBaseModifier(body)
    {
        public override string MeetingMessage()
        {
            return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerKills{CoronerData?.KillerStatus.ToString() ?? ""}").Replace("<count>", $"{CoronerData?.KillerOtherKills ?? 0}");
        }
        public override string NotificationMessage()
        {
            return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerKills{CoronerData?.KillerStatus.ToString() ?? ""}").Replace("<count>", $"{CoronerData?.KillerOtherKills ?? 0}");
        }
    }
}
