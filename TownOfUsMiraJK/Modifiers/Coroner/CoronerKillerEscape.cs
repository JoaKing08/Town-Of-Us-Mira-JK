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
