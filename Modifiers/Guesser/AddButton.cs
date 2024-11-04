using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using MiraAPI.Utilities.Assets;
using System.Reflection;
using Reactor.Utilities.Extensions;
using System;
using TMPro;
using yanplaRoles.Roles.Crewmate;

namespace yanplaRoles.Modifiers.Guesser;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public class AddButton
{
    private static Assembly assembly = Assembly.GetExecutingAssembly();
    private static LoadableAsset<Sprite> CycleBackSprite => Assets.CycleBack;
    private static LoadableAsset<Sprite> CycleForwardSprite => Assets.CycleForward;

    private static LoadableAsset<Sprite> GuessSprite => Assets.Guess;

    private static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead) return true;
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (
                player == null ||
                player.Data.Role.IsImpostor ||
                player.Data.IsDead ||
                player.Data.Disconnected
            ) return true;
            else if (
                PlayerControl.LocalPlayer.Data.Role.IsImpostor &&
                player.Data.Role is Snitch snitch && 
                snitch.revealed
            ) return true;
            return player.Data.Role == null;
        }

    public static void GenButton(Guesser modifier, PlayerVoteArea voteArea)
        {
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea))
            {
                modifier.Buttons[targetId] = (null, null, null, null);
                return;
            }

            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var parent = confirmButton.transform.parent.parent;
            
            var nameText = UnityEngine.Object.Instantiate(voteArea.NameText, voteArea.transform);
            voteArea.NameText.transform.localPosition = new Vector3(0.55f, 0.12f, -0.1f);
            nameText.transform.localPosition = new Vector3(0.55f, -0.12f, -0.1f);
            nameText.text = "Guess";

            var cycleBack = UnityEngine.Object.Instantiate(confirmButton, voteArea.transform);
            var cycleRendererBack = cycleBack.GetComponent<SpriteRenderer>();
            cycleRendererBack.sprite = CycleBackSprite.LoadAsset();
            cycleBack.transform.localPosition = new Vector3(-0.5f, 0.15f, -2f);
            cycleBack.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            cycleBack.layer = 5;
            cycleBack.transform.parent = parent;
            var cycleEventBack = new Button.ButtonClickedEvent();
            cycleEventBack.AddListener(Cycle(modifier, voteArea, nameText, false));
            cycleBack.GetComponent<PassiveButton>().OnClick = cycleEventBack;
            var cycleColliderBack = cycleBack.GetComponent<BoxCollider2D>();
            cycleColliderBack.size = cycleRendererBack.sprite.bounds.size;
            cycleColliderBack.offset = Vector2.zero;
            cycleBack.transform.GetChild(0).gameObject.Destroy();

            var cycleForward = UnityEngine.Object.Instantiate(confirmButton, voteArea.transform);
            var cycleRendererForward = cycleForward.GetComponent<SpriteRenderer>();
            cycleRendererForward.sprite = CycleForwardSprite.LoadAsset();
            cycleForward.transform.localPosition = new Vector3(-0.2f, 0.15f, -2f);
            cycleForward.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            cycleForward.layer = 5;
            cycleForward.transform.parent = parent;
            var cycleEventForward = new Button.ButtonClickedEvent();
            cycleEventForward.AddListener(Cycle(modifier, voteArea, nameText, true));
            cycleForward.GetComponent<PassiveButton>().OnClick = cycleEventForward;
            var cycleColliderForward = cycleForward.GetComponent<BoxCollider2D>();
            cycleColliderForward.size = cycleRendererForward.sprite.bounds.size;
            cycleColliderForward.offset = Vector2.zero;
            cycleForward.transform.GetChild(0).gameObject.Destroy();

            var guess = UnityEngine.Object.Instantiate(confirmButton, voteArea.transform);
            var guessRenderer = guess.GetComponent<SpriteRenderer>();
            guessRenderer.sprite = GuessSprite.LoadAsset();
            guess.transform.localPosition = new Vector3(-0.35f, -0.15f, -2f);
            guess.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            guess.layer = 5;
            guess.transform.parent = parent;
            var guessEvent = new Button.ButtonClickedEvent();
            guessEvent.AddListener(Guess(modifier, voteArea));
            guess.GetComponent<PassiveButton>().OnClick = guessEvent;
            var bounds = guess.GetComponent<SpriteRenderer>().bounds;
            bounds.size = new Vector3(0.52f, 0.3f, 0.16f);
            var guessCollider = guess.GetComponent<BoxCollider2D>();
            guessCollider.size = guessRenderer.sprite.bounds.size;
            guessCollider.offset = Vector2.zero;
            guess.transform.GetChild(0).gameObject.Destroy();

            modifier.Guesses.Add(targetId, "None");
            modifier.Buttons[targetId] = (cycleBack, cycleForward, guess, nameText);
        }

        private static Action Cycle(Guesser modifier, PlayerVoteArea voteArea, TextMeshPro nameText, bool forwardsCycle = true)
        {
            void Listener()
            {
                if (MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion) return;
                var currentGuess = modifier.Guesses[voteArea.TargetPlayerId];
                var guessIndex = currentGuess == "None"
                    ? -1
                    : modifier.PossibleGuesses.IndexOf(currentGuess);
                if (forwardsCycle)
                {
                    if (++guessIndex >= modifier.PossibleGuesses.Count)
                        guessIndex = 0;
                }
                else
                {
                    if (--guessIndex < 0)
                        guessIndex = modifier.PossibleGuesses.Count - 1;
                }

                var newGuess = modifier.Guesses[voteArea.TargetPlayerId] = modifier.PossibleGuesses[guessIndex];

                nameText.text = newGuess == "None"
                    ? "Guess"
                    : $"<color=#{modifier.SortedColorMapping[newGuess].ToHtmlStringRGBA()}>{newGuess}</color>";
            }

            return Listener;
        }

        private static Action Guess(Guesser modifier, PlayerVoteArea voteArea)
        {
            void Listener()
            {
                if (
                    MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion ||
                    IsExempt(voteArea) || PlayerControl.LocalPlayer.Data.IsDead
                ) return;
                var targetId = voteArea.TargetPlayerId;
                var currentGuess = modifier.Guesses[targetId];
                if (currentGuess == "None") return;

                var player = Utils.PlayerById(targetId);
                var playerRole = player.Data.Role;

                var toDie = playerRole.NiceName == currentGuess ? playerRole.Player : modifier.Player;

                GuesserKill.RpcMurderPlayer(toDie, PlayerControl.LocalPlayer);
                modifier.RemainingKills--;
                ShowHideButtons.HideSingle(modifier, targetId, toDie == modifier.Player);
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            var players = PlayerControl.AllPlayerControls;
            foreach (var player in players)
            {
                if (MiraAPI.Utilities.Extensions.HasModifier<Guesser>(player))
                {
                    var assassin = MiraAPI.Utilities.Extensions.GetModifier<Guesser>(player);
                    assassin.Guesses.Clear();
                    assassin.Buttons.Clear();
                    assassin.GuessedThisMeeting = false;
                }
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!MiraAPI.Utilities.Extensions.HasModifier<Guesser>(PlayerControl.LocalPlayer)) return;

            var guesser = MiraAPI.Utilities.Extensions.GetModifier<Guesser>(PlayerControl.LocalPlayer);
            if (guesser.RemainingKills <= 0) return;
            foreach (var voteArea in __instance.playerStates)
            {
                GenButton(guesser, voteArea);
            }
        }
}