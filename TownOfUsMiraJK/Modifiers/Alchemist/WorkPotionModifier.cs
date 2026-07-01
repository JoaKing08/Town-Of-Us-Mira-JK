using Il2CppSystem.Text;
using System.Text.RegularExpressions;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers.Alchemist;

public sealed class WorkPotionModifier(PlayerControl alchemist) : TimedModifier, IPotionEffect
{
    public override string ModifierName => TouLocale.Get("TouJKRoleAlchemistPotionWorkPotion");
    public bool CanAppear => true;
    public override bool AutoStart => true;
    public override float Duration => 0.1f;
    public bool CanSelf => OptionGroupSingleton<AlchemistOptions>.Instance.AllowSelf;
    public string PotionName => TouLocale.Get("TouJKRoleAlchemistPotionWorkPotion");
    public PlayerControl Alchemist => alchemist;

    public override void OnActivate()
    {
        base.OnActivate();
        if (Player.AmOwner && Player.myTasks.Count > 0 && !Player.HasDied() && Player.IsCrewmate())
        {
            for (int i = 0; i < OptionGroupSingleton<AlchemistOptions>.Instance.WorkPotion; i++)
            {
                var tasks = Player.myTasks.ToArray().Where(x => x.TryCast<NormalPlayerTask>() != null && !x.IsComplete)
                    .ToList();

                if (tasks.Count > 0)
                {
                    tasks.Shuffle();

                    var randomTask = tasks[0];

                    HudManager.Instance.ShowTaskComplete();
                    Player.RpcCompleteTask(randomTask.Id);

                    var sb = new StringBuilder();
                    randomTask.AppendTaskText(sb);
                }
            }
        }
    }
}