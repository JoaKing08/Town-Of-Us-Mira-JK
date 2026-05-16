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
    public sealed class CoronerKillerRole(DeadBody body) : CoronerResultBaseModifier(body)
    {
        public override string MeetingMessage()
        {
            if (CoronerData?.VictimId == CoronerData?.KillerId)
            {
                return TouLocale.GetParsed("TouJKRoleCoronerAutopsyKillerRoleSuicide");
            }
            RoleBehaviour role = CustomRoleUtils.GetRegisteredRole(CoronerData?.KillerRole ?? AmongUs.GameOptions.RoleTypes.Crewmate)!;
            return TouLocale.GetParsed("TouJKRoleCoronerAutopsyKillerRole").Replace("<role>", $"{role.TeamColor.ToTextColor()}{role.GetRoleName()}</color>");
        }
        public override string NotificationMessage()
        {
            if (CoronerData?.VictimId == CoronerData?.KillerId)
            {
                return TouLocale.GetParsed("TouJKRoleCoronerAutopsyKillerRoleSuicide");
            }
            RoleBehaviour role = CustomRoleUtils.GetRegisteredRole(CoronerData?.KillerRole ?? AmongUs.GameOptions.RoleTypes.Crewmate)!;
            return TouLocale.GetParsed("TouJKRoleCoronerAutopsyKillerRole").Replace("<role>", $"{role.TeamColor.ToTextColor()}{role.GetRoleName()}</color>");
        }
    }
}
