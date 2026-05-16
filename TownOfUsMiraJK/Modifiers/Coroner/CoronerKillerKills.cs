using MiraAPI.Roles;
using MiraAPI.Utilities;
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
