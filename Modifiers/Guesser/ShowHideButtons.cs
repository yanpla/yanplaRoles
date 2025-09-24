using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using UnityEngine.UI;
using yanplaRoles.Options.Modifiers;

namespace yanplaRoles.Modifiers.Guesser;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
public class ShowHideButtons
{
    public static void HideButtons(Guesser modifier)
    {
        foreach (var (_, (cycleBack, cycleForward, guess, guessText)) in modifier.Buttons)
        {
            if (cycleBack == null || cycleForward == null) continue;
            cycleBack.SetActive(false);
            cycleForward.SetActive(false);
            guess.SetActive(false);
            guessText.gameObject.SetActive(false);

            cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            modifier.GuessedThisMeeting = true;
        }
    }

    public static void HideSingle(
        Guesser modifier,
        byte targetId,
        bool killedSelf
    )
    {
        if (killedSelf || modifier.RemainingKills == 0 || !OptionGroupSingleton<GuesserOptions>.Instance.GuessMultiple) HideButtons(modifier);
        else HideTarget(modifier, targetId);
    }
    public static void HideTarget(
        Guesser modifier,
        byte targetId
    )
    {

        var (cycleBack, cycleForward, guess, guessText) = modifier.Buttons[targetId];
        if (cycleBack == null || cycleForward == null) return;
        cycleBack.SetActive(false);
        cycleForward.SetActive(false);
        guess.SetActive(false);
        guessText.gameObject.SetActive(false);

        cycleBack.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
        cycleForward.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
        guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
        modifier.Buttons[targetId] = (null, null, null, null);
        modifier.Guesses.Remove(targetId);
    }


    public static void Prefix(MeetingHud __instance)
    {
        if (!PlayerControl.LocalPlayer.HasModifier<Guesser>()) return;
        var guesser = PlayerControl.LocalPlayer.GetModifier<Guesser>();
        if (!OptionGroupSingleton<GuesserOptions>.Instance.GuessAfterVoting) HideButtons(guesser);
    }
}
