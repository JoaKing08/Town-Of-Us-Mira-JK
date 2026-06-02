using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using TownOfUs;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Neutral;
using TownOfUs.Modules.Localization;
using TownOfUs.Utilities;
using TownOfUsMiraJK.Assets;
using TownOfUsMiraJK.Enums;
using TownOfUsMiraJK.Options.Roles.Impostor;
using TownOfUsMiraJK.Roles.Impostor;
using TownOfUsMiraJK.Utilities;
using UnityEngine;

namespace TownOfUsMiraJK.Modifiers
{
    public sealed class DemagogueRevealModifier()
    : BaseRevealModifier
    {
        public static SpriteRenderer Tint;
        public override string ModifierName => "Reveal";
        public override bool RevealRole { get; set; } = true;
        public override RoleBehaviour? ShownRole => CustomRoleUtils.GetRegisteredRole((RoleTypes)RoleId.Get<DemagogueRole>());
        public override ChangeRoleResult ChangeRoleResult => ChangeRoleResult.RemoveModifier;
        public bool FirstMeeting { get; set; } = true;
        public List<byte> Hinted { get; set; }
        public bool DeathRevealed { get; set; }
        public override void OnActivate()
        {
            base.OnActivate();
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleDemagogueNotif").Replace("<player>", $"{TownOfUsColors.Impostor.ToTextColor()}{Player.Data.PlayerName}</color>").Replace("<role>", $"{TownOfUsColors.Impostor.ToTextColor()}{ShownRole?.GetRoleName()}</color>").Replace("<alignment>", MiscUtils.GetParsedRoleAlignment((Player.Data.Role as DemagogueRole)?.Immunity?.Data.Role ?? Player.Data.Role, true)),
                Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Demagogue.LoadAsset());

            notif1.AdjustNotification();
            MiscUtils.AddFakeChat(Player.Data, TouLocale.GetParsed("TouJKRoleDemagogueNotifTitle"), TouLocale.GetParsed("TouJKRoleDemagogueNotif").Replace("<player>", Player.Data.PlayerName).Replace("<role>", MiscUtils.GetHyperlinkText(Player.Data.Role)).Replace("<alignment>", MiscUtils.GetParsedRoleAlignment((Player.Data.Role as DemagogueRole)?.Immunity?.Data.Role ?? Player.Data.Role, true)));
            Hinted = ModifierUtils.GetPlayersWithModifier<DemagogueImmunityModifier>(x => x.OwnerId == Player.PlayerId).Select(x => x.PlayerId).ToList();
            Hinted.Add(Player.PlayerId);
            OnMeetingStart();
        }
        public override void OnMeetingStart()
        {
            base.OnMeetingStart();
            if (Tint == null)
            {
                Tint = UnityEngine.Object.Instantiate(HudManager.Instance.FullScreen, HudManager.Instance.FullScreen.transform.parent);
                Tint.color = TownOfUsColors.Impostor.SetAlpha(0.1f);
            }
            Tint.gameObject.SetActive(!Player.HasDied());
            var opts = OptionGroupSingleton<DemagogueOptions>.Instance;
            if (!DeathRevealed)
            {
                if (opts.AnnounceImmunityDeath && Player.GetRole<DemagogueRole>()?.ImmunityAlive == false)
                {
                    var notif1 = Helpers.CreateAndShowNotification(
                        TouLocale.GetParsed("TouJKRoleDemagogueImmunityDeathNotif").Replace("<player>", $"{TownOfUsColors.Impostor.ToTextColor()}{Player.Data.PlayerName}</color>").Replace("<role>", $"{TownOfUsColors.Impostor.ToTextColor()}{ShownRole?.GetRoleName()}</color>"),
                        Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Demagogue.LoadAsset());

                    notif1.AdjustNotification();
                    MiscUtils.AddFakeChat(Player.Data, TouLocale.GetParsed("TouJKRoleDemagogueImmunityDeathNotifTitle"), TouLocale.GetParsed("TouJKRoleDemagogueImmunityDeathNotif").Replace("<player>", Player.Data.PlayerName).Replace("<role>", MiscUtils.GetHyperlinkText(Player.Data.Role)));
                    DeathRevealed = true;
                }
                else if (Player.AmOwner && !FirstMeeting && opts.GiveHints)
                {
                    var hints = Helpers.GetAlivePlayers().Where(x => !Hinted.Contains(x.PlayerId));
                    hints.Shuffle();
                    var hint = hints.FirstOrDefault();
                    if (hint != null)
                    {
                        RpcDemagogueHint(Player, hint);
                    }
                }
            }
            FirstMeeting = false;
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (Tint != null)
            {
                UnityEngine.Object.Destroy(Tint.gameObject);
            }
        }

        [MethodRpc((uint)TownOfUsJKRpc.DemagogueHint)]
        public static void RpcDemagogueHint(PlayerControl hinting, PlayerControl hinted)
        {
            if (LobbyBehaviour.Instance)
            {
                MiscUtils.RunAnticheatWarning(hinting);
                return;
            }
            var notif1 = Helpers.CreateAndShowNotification(
                TouLocale.GetParsed("TouJKRoleDemagogueHint").Replace("<player>", $"{TownOfUsColors.Impostor.ToTextColor()}{hinted.Data.PlayerName}</color>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: RoleIcons.Demagogue.LoadAsset());

            notif1.AdjustNotification();
            MiscUtils.AddFakeChat(hinted.Data, TouLocale.GetParsed("TouJKRoleDemagogueHintTitle"), TouLocale.GetParsed("TouJKRoleDemagogueHint").Replace("<player>", hinted.Data.PlayerName));
            hinting.GetModifier<DemagogueRevealModifier>()?.Hinted.Add(hinted.PlayerId);
        }
    }
}