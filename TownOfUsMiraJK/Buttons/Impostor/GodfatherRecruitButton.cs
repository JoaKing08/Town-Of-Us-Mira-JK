using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Buttons;
using TownOfUs.Buttons.Impostor;
using TownOfUs.Interfaces.BaseGame;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules.Localization;
using TownOfUs.Options;
using TownOfUs.Options.Maps;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Options.Roles.Impostor;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Impostor;
using UnityEngine;
using static Sentry.MeasurementUnit;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUsMiraJK.Buttons.Impostor;

public sealed class GodfatherRecruitButton : TownOfUsRoleButton<GodfatherRole, PlayerControl>
{
    public override string Name => TouLocale.GetParsed("TouJKRoleGodfatherRecruit", "Recruit");
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Impostor;
    public override float Cooldown => 0.001f;
    public override LoadableAsset<Sprite> Sprite => ImpAssets.GodfatherRecruitSprite;
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && role is GodfatherRole godfather && !godfather.Recruited;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        if (Target.IsCrewmate() && !Target.HasModifier<AllianceGameModifier>())
        {
            GodfatherRole.RpcGodfatherRecruit(PlayerControl.LocalPlayer, Target);
            SetActive(false, Role);
        }
        else
        {
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleGodfatherRecruitFailedNotif").Replace("<player>", $"{TownOfUsColors.Impostor.ToTextColor()}{Target.Data.PlayerName}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Godfather.LoadAsset());

            notif1.AdjustNotification();
        }
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance, predicate: x => !x.IsRole<UndercoverRole>() && !x.HasModifier<AllianceGameModifier>(x => x.TrueFactionType == AlliedFaction.Impostor));
    }
}