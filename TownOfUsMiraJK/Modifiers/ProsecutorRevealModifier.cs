using MiraAPI.GameOptions;
using TownOfUs.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class ProsecutorRevealModifier(RoleBehaviour role)
    : BaseRevealModifier
{
    public override string ModifierName => "Prosecutor Reveal";

    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.RemoveModifier;

    public override RoleBehaviour? ShownRole { get; set; } = role;
    public override bool RevealRole { get; set; } = true;

    public override void OnDeath(DeathReason reason)
    {
        base.OnDeath(reason);
        ModifierComponent?.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Player.Data.Role is ProsecutorRole Gunslinger)
        {
            Visible = Gunslinger.ProsecutionsCompleted > 0 && OptionGroupSingleton<ProsecutorJKOptions>.Instance.Reveal;
        }
        else
        {
            Visible = false;
        }
    }
}