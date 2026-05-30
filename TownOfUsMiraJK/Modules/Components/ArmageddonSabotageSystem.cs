using Hazel;
using Il2CppInterop.Runtime.Injection;
using MiraAPI.Roles;
using Reactor.Utilities.Attributes;
using TownOfUs.Events;
using TownOfUs.Modifiers;
using TownOfUs.Modules.Localization;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Roles.Neutral;
using TownOfUsMiraJK.Utilities;

namespace TownOfUsMiraJK.Modules.Components;

[RegisterInIl2Cpp(typeof(ISystemType), typeof(IActivatable))]
public sealed class ArmageddonSabotageSystem(nint cppPtr) : Il2CppSystem.Object(cppPtr)
{
    public const byte SabotageId = 170;
    public readonly float duration;

    public bool IsActive => (TimeRemaining > 0 || Stage == ArmageddonStage.Finished);
    public static bool InMeeting => MeetingHud.Instance != null || ExileController.Instance != null;
    public bool IsDirty { get; private set; }
    public float TimeRemaining { get; private set; }
    public ArmageddonStage Stage { get; private set; }
    public static bool ArmageddonFinished { get; internal set; }
    public static bool RoundStart { get; set; }

    private float _dirtyTimer;
    public ArmageddonSabotageSystem(float duration) : this(ClassInjector.DerivedConstructorPointer<ArmageddonSabotageSystem>())
    {
        ClassInjector.DerivedConstructorBody(this);
        Instance = this;
        this.duration = duration;
    }

    public static ArmageddonSabotageSystem Instance { get; private set; }
    public void Deteriorate(float deltaTime)
    {
        if (!IsActive)
        {
            if (Stage != ArmageddonStage.None)
            {
                Stage = ArmageddonStage.None;
                IsDirty = true;
                ArmageddonFinished = false;
            }
            RoundStart = false;

            return;
        }

        if (InMeeting)
        {
            return;
        }

        if (!PlayerTask.PlayerHasTaskOfType<ArmageddonSabotageTask>(PlayerControl.LocalPlayer))
        {
            PlayerControl.LocalPlayer.AddSystemTask((SystemTypes)SabotageId);
        }

        if (!InMeeting)
        {
            TimeRemaining -= deltaTime;
            _dirtyTimer += deltaTime;
            
            if (_dirtyTimer > 2f)
            {
                _dirtyTimer = 0f;
                IsDirty = true;
            }
        }
        
        if (RoundStart)
        {
            TimeRemaining = 0;
        }
        if (TimeRemaining <= 0)
        {
            if (Stage == ArmageddonStage.Initiate)
            {
                Stage = ArmageddonStage.Countdown;
                TimeRemaining = duration;
                ArmageddonFinished = false;
                IsDirty = true;
            }
            else if (Stage == ArmageddonStage.Countdown)
            {
                Stage = ArmageddonStage.Finished;
                var death = CustomRoleUtils.GetActiveRolesOfType<DeathRole>().FirstOrDefault();
                if (death != null)
                {
                    foreach (var player in PlayerControl.AllPlayerControls.ToArray()
                                 .Where(x => !x.HasDied() && !x.IsApocalypseAligned()))
                    {
                        DeathHandlerModifier.UpdateDeathHandlerImmediate(player, TouLocale.Get("DiedToDeathArmageddon"), DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetTrue,
                            TouLocale.GetParsed("DiedByStringBasic").Replace("<player>", death.Player.Data.PlayerName),
                            lockInfo: DeathHandlerOverride.SetTrue);
                    }
                }
                TimeRemaining = 7f;
                ArmageddonFinished = false;
                IsDirty = true;
            }
            else if (Stage == ArmageddonStage.DeathDead)
            {
                IsDirty = true;
                Stage = ArmageddonStage.None;

                ArmageddonFinished = false;
            }
            else if (Stage == ArmageddonStage.Finished)
            {
                IsDirty = true;
                if (TutorialManager.InstanceExists)
                {
                    TimeRemaining = 7f;
                    Stage = ArmageddonStage.DeathDead;
                }
                ArmageddonFinished = true;
            }
        }
        else if (Stage == ArmageddonStage.Countdown && !CustomRoleUtils.GetActiveRolesOfType<DeathRole>().HasAny())
        {
            Stage = ArmageddonStage.DeathDead;
            TimeRemaining = 3f;
            ArmageddonFinished = false;
            IsDirty = true;
        }
        RoundStart = false;
    }

    public void UpdateSystem(PlayerControl player, MessageReader msgReader)
    {
        if (msgReader.ReadByte() != 1) return;
        Stage = ArmageddonStage.Initiate;
        TimeRemaining = 4f;
        IsDirty = true;
    }

    public void Deserialize(MessageReader reader, bool initialState)
    {
        TimeRemaining = reader.ReadSingle();
        Stage = (ArmageddonStage)reader.ReadByte();
    }

    public void Serialize(MessageWriter writer, bool initialState)
    {
        writer.Write(TimeRemaining);
        writer.Write((byte)Stage);
        IsDirty = initialState;
    }

    public void MarkClean()
	{
		IsDirty = false;
	}
}

public enum ArmageddonStage
{
    Initiate,
    None,
    Countdown,
    Finished,
    DeathDead,
}
