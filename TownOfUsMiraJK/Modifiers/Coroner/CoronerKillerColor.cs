using MS.Internal.Xml.XPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
