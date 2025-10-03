namespace Codex.Domain.Battles;

using System;
using System.Collections.Generic;
using System.Linq;
using Codex.Domain.Guardians;
using Codex.Domain.Teams;

/// <summary>
/// Basisklasse voor gevechten. Houdt id, datum/tijd en locatie (venue) bij.
/// </summary>
public abstract class Battle
{
    public int Id { get; }
    public DateTime DateTime { get; }
    public string Venue { get; }

    protected Battle(int id, DateTime dateTime, string venue)
    {
        Id = id;
        DateTime = dateTime;
        Venue = venue;
    }
}

/// <summary>
/// Individueel gevecht tussen twee Guardians van verschillend type.
/// </summary>
public sealed class IndividualBattle : Battle
{
    public Guardian A { get; }
    public Guardian B { get; }
    public Guardian Winner { get; }
    public Guardian Loser => ReferenceEquals(Winner, A) ? B : A;

    public IndividualBattle(int id, DateTime dateTime, string venue, Guardian a, Guardian b, Guardian winner)
        : base(id, dateTime, venue)
    {
        if (a.Type == b.Type) throw new InvalidOperationException("Individual battles must be between different guardian types.");
        if (!ReferenceEquals(winner, a) && !ReferenceEquals(winner, b)) throw new ArgumentException("Winner must be one of the participants");
        A = a; B = b; Winner = winner;
    }
}

/// <summary>
/// Teamgevecht tussen twee teams van verschillende Guardian-typen.
/// Teams moeten geldig zijn (minimaal twee leden).
/// </summary>
public sealed class TeamBattle : Battle
{
    public Team TeamA { get; }
    public Team TeamB { get; }
    public Team Winner { get; }

    public TeamBattle(int id, DateTime dateTime, string venue, Team teamA, Team teamB, Team winner)
        : base(id, dateTime, venue)
    {
        if (!teamA.IsValidTeam() || !teamB.IsValidTeam()) throw new InvalidOperationException("Teams must have at least two members.");
        if (teamA.TeamType == null || teamB.TeamType == null || teamA.TeamType == teamB.TeamType)
            throw new InvalidOperationException("Teams must be of different guardian types.");
        if (!ReferenceEquals(winner, teamA) && !ReferenceEquals(winner, teamB)) throw new ArgumentException("Winner must be one of the teams");
        TeamA = teamA; TeamB = teamB; Winner = winner;
    }
}


