using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfUs.Modules;
using TownOfUs.Roles.Crewmate;
using TownOfUsMiraJK.Options;
using TownOfUsMiraJK.Options.Roles;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class ProsecutorRevealModifier(RoleBehaviour role)
    : BaseRevealModifier
{
    public override string ModifierName => "Prosecutor Reveal";

    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.RemoveModifier;

    public override RoleBehaviour? ShownRole { get; set; } = role;
    public override bool RevealRole { get; set; } = true;

    public override void OnActivate()
    {
        if (!Player.AmOwner && Player.Data.Role is ProsecutorRole Prosecutor && Prosecutor.ProsecutionsCompleted > 0 && OptionGroupSingleton<TouMTweaksOptions>.Instance.ProsecutorReveal)
        {
            MeetingMenu.Instances.Do(x => x.HideSingle(Player.PlayerId));
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        base.OnDeath(reason);
        ModifierComponent?.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Visible = Player.Data.Role is ProsecutorRole Prosecutor && Prosecutor.ProsecutionsCompleted > 0 && OptionGroupSingleton<TouMTweaksOptions>.Instance.ProsecutorReveal;
    }
}