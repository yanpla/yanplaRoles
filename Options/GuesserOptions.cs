using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace yanplaRoles.Options.Modifiers;

public class GuesserOptions : AbstractOptionGroup
{ 
    public override string GroupName => "Guesser";
    public override Color GroupColor => Palette.ImpostorRed;

    [ModdedNumberOption("Number of Guessers", min: 0, max: 5, 1f)]
    public float NumberOfGuessers { get; set; } = 1f;

    [ModdedNumberOption("Possible Guesses", min: 1, max: 15, 1f)]
    public float PossibleGuesses { get; set; } = 1f;

    [ModdedToggleOption("Guess Crewmate")]
    public bool GuessCrewmate { get; set; } = false;

    [ModdedToggleOption("Guess Multiple")]
    public bool GuessMultiple { get; set; } = false;

    [ModdedToggleOption("Guess After Voting")]
    public bool GuessAfterVoting { get; set; } = false;
}