using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers
{
    public sealed class WitchRevealModifier(ushort roleId)
    : BaseRevealModifier
    {
        public override string ModifierName => "Witch Learns";
        public override bool HideOnUi => true;

        public override bool RevealRole { get; set; } = true;
        public override void OnActivate()
        {
            base.OnActivate();
            ShownRole = CustomRoleUtils.GetRegisteredRole((RoleTypes)roleId);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Visible = PlayerControl.LocalPlayer.IsRole<WitchRole>();
            if (Player.HasModifier<BaseRevealModifier>(x => x.Visible && x is not WitchRevealModifier))
            {
                ModifierComponent?.RemoveModifier(this);
            }
        }
    }
}
