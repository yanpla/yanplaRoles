using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using yanplaRoles.Options.Modifiers;
﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using MiraAPI.Roles;
using yanplaRoles.Roles.Crewmate;
using yanplaRoles.Roles.Neutral;
using yanplaRoles.Roles.Impostor;

namespace yanplaRoles.Modifiers.Guesser;

[RegisterModifier]
public class Guesser : GameModifier
{
    public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; } = new();

    private Dictionary<string, Color> ColorMapping = new();
    public Dictionary<string, Color> SortedColorMapping;

    public Dictionary<byte, string> Guesses = new();

    public override string ModifierName => "Guesser";

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role.TeamType == RoleTeamTypes.Impostor;
    }
    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<GuesserOptions>.Instance.NumberOfGuessers;
    }

    public override int GetAssignmentChance()
    {
        return 100;
    }

    public Guesser()
    {
        // CHANGE THIS IF TOO MANY ROLES
        if (CustomRoleSingleton<Snitch>.Instance.GetCount() > 0) ColorMapping.Add("Snitch", CustomRoleSingleton<Snitch>.Instance.RoleColor);
        if (CustomRoleSingleton<Sheriff>.Instance.GetCount() > 0) ColorMapping.Add("Sheriff", CustomRoleSingleton<Sheriff>.Instance.RoleColor);
        if (CustomRoleSingleton<Jester>.Instance.GetCount() > 0) ColorMapping.Add("Jester", CustomRoleSingleton<Jester>.Instance.RoleColor);
        if (CustomRoleSingleton<Executioner>.Instance.GetCount() > 0) ColorMapping.Add("Executioner", CustomRoleSingleton<Executioner>.Instance.RoleColor);
        var roleOptions = GameOptionsManager.Instance.currentGameOptions.RoleOptions;
        if (roleOptions.GetChancePerGame(AmongUs.GameOptions.RoleTypes.Engineer) > 0) ColorMapping.Add("Engineer", Palette.CrewmateBlue);
        if (roleOptions.GetChancePerGame(AmongUs.GameOptions.RoleTypes.Tracker) > 0) ColorMapping.Add("Tracker", Palette.CrewmateBlue);
        if (roleOptions.GetChancePerGame(AmongUs.GameOptions.RoleTypes.Scientist) > 0) ColorMapping.Add("Scientist", Palette.CrewmateBlue);
        if (roleOptions.GetChancePerGame(AmongUs.GameOptions.RoleTypes.Noisemaker) > 0) ColorMapping.Add("Noisemaker", Palette.CrewmateBlue);
        if (OptionGroupSingleton<GuesserOptions>.Instance.GuessCrewmate) ColorMapping.Add("Crewmate", Palette.CrewmateBlue);

        SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
    }

    public bool GuessedThisMeeting { get; set; } = false;

    public int RemainingKills { get; set; } = (int)OptionGroupSingleton<GuesserOptions>.Instance.PossibleGuesses;

    public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();

    protected internal int CorrectAssassinKills { get; set; } = 0;
    protected internal int IncorrectAssassinKills { get; set; } = 0;
}