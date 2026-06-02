using TownOfUs.Modifiers.Crewmate.Coroner;
using TownOfUs.Modules.Localization;

namespace TownOfUsMiraJK.Modifiers.Coroner
{
    public sealed class CoronerKillIndirect(DeadBody body) : CoronerResultBaseModifier(body)
    {
        public override string MeetingMessage()
        {
            if (CoronerData?.IsIndirect == true)
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillIndirectTrue");
            }
            else
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillIndirectFalse");
            }
        }
        public override string NotificationMessage()
        {
            if (CoronerData?.IsIndirect == true)
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillIndirectTrue");
            }
            else
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillIndirectFalse");
            }
        }
    }
}
