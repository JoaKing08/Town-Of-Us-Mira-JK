using TownOfUs.Modifiers.Crewmate.Coroner;
using TownOfUs.Modules.Localization;

namespace TownOfUsMiraJK.Modifiers.Coroner
{
    public sealed class CoronerKillerEscapeMethod(DeadBody body) : CoronerResultBaseModifier(body)
    {
        public override string MeetingMessage()
        {
            if (CoronerData?.IsIndirect == true)
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscapeMethodIndirect");
            }
            else if (CoronerData?.AbilityEscape == true)
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscapeMethodAbility");
            }
            else if (CoronerData?.Vented == true)
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscapeMethodVent");
            }
            else
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscapeMethodNormal");
            }
        }
        public override string NotificationMessage()
        {
            if (CoronerData?.IsIndirect == true)
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscapeMethodIndirect");
            }
            else if (CoronerData?.AbilityEscape == true)
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscapeMethodAbility");
            }
            else if (CoronerData?.Vented == true)
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscapeMethodVent");
            }
            else
            {
                return TouLocale.GetParsed($"TouJKRoleCoronerAutopsyKillerEscapeMethodNormal");
            }
        }
    }
}
