using MiraAPI.GameOptions;
using TownOfUs.Roles.Crewmate;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using TownOfUsMiraJK.Roles.Crewmate;

namespace TownOfUs.Modifiers.Crewmate;

public sealed class ExecutorRevealModifier(RoleBehaviour role)
    : BaseRevealModifier
{
    public override string ModifierName => "Executor Reveal";

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
        if (Player.Data.Role is ExecutorRole executor)
        {
            Visible = executor.HasExecuted && OptionGroupSingleton<ExecutorOptions>.Instance.Reveal;
        }
        else
        {
            Visible = false;
        }
    }
}