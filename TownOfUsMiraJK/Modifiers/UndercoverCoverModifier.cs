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
using TownOfUsMiraJK.Modifiers.Game.Impostor;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers
{
    public sealed class UndercoverCoverModifier(ushort coverId)
    : BaseRevealModifier
    {
        public override LoadableAsset<Sprite>? ModifierIcon => RoleIcons.Undercover;
        public override string ModifierName => "Cover";
        public override bool HideOnUi => false;

        public override string GetDescription()
        {
            return TouLocale.GetParsed("TouJKRoleUndercoverTabCoverInfo").Replace("<role>", $"{ShownRole.NameColor.ToTextColor()}{ShownRole.GetRoleName()}</color>");
        }

        public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.RemoveModifier;
        public override bool RevealRole { get; set; } = true;
        public override void OnActivate()
        {
            base.OnActivate();
            ShownRole = CustomRoleUtils.GetRegisteredRole((RoleTypes)coverId);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Visible = (OptionGroupSingleton<GeneralOptions>.Instance.ImpsKnowRoles && PlayerControl.LocalPlayer.IsImpostorAligned() && !PlayerControl.LocalPlayer.HasModifier<OutcastModifier>()) || (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow && PlayerControl.LocalPlayer.DiedOtherRound());
        }
    }
}
