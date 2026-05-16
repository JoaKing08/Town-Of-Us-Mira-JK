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
    public sealed class CoronerVictimRole(DeadBody body) : CoronerResultBaseModifier(body)
    {
        public override string MeetingMessage()
        {
            RoleBehaviour role = CustomRoleUtils.GetRegisteredRole(CoronerData?.VictimRole ?? AmongUs.GameOptions.RoleTypes.Crewmate)!;
            return TouLocale.GetParsed("TouJKRoleCoronerAutopsyVictimRole").Replace("<role>", $"{role.TeamColor.ToTextColor()}{role.GetRoleName()}</color>");
        }
        public override string NotificationMessage()
        {
            RoleBehaviour role = CustomRoleUtils.GetRegisteredRole(CoronerData?.VictimRole ?? AmongUs.GameOptions.RoleTypes.Crewmate)!;
            return TouLocale.GetParsed("TouJKRoleCoronerAutopsyVictimRole").Replace("<role>", $"{role.TeamColor.ToTextColor()}{role.GetRoleName()}</color>");
        }
    }
}
