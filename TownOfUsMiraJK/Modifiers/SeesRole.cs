using AmongUs.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Modifiers;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Modifiers.Game.Alliance;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;

namespace TownOfUsMiraJK.Modifiers
{
    public sealed class SeesRole()
    : BaseRevealModifier
    {
        public override string ModifierName => "Sees Role";

        public override bool RevealRole { get; set; } = true;
    }
}