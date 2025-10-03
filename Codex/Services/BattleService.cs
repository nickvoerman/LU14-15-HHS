namespace Codex.Services;

using System;
using System.Collections.Generic;
using Codex.Domain.Battles;
using Codex.Domain.Guardians;
using Codex.Domain.Teams;

/// <summary>
/// Bepaalt winnaars van gevechten. Maakt gebruik van PowerCalculator en een injecteerbare
/// fan-teller. De randomness zorgt ervoor dat de sterkere partij een grotere kans heeft,
/// maar niet altijd wint.
/// </summary>
public class BattleService
{
    private readonly Func<Guardian, int> _fanCountProvider;
    private readonly Random _random;

    /// <summary>
    /// Injecteer een functie die het aantal fans per Guardian aanlevert en optioneel een Random
    /// voor reproduceerbare tests.
    /// </summary>
    public BattleService(Func<Guardian, int> fanCountProvider, Random? random = null)
    {
        _fanCountProvider = fanCountProvider;
        _random = random ?? new Random();
    }

    /// <summary>
    /// Bepaal de winnaar van een individueel gevecht op basis van gewogen random keuze.
    /// Aartsvijand-bonus wordt toegepast via ApplyFanAndArchEnemyBonuses.
    /// </summary>
    public Guardian CalculateWinner(IndividualBattle battle)
    {
        var aScore = PowerCalculator.ApplyFanAndArchEnemyBonuses(
            PowerCalculator.CalculateGuardianCombatScore(battle.A),
            _fanCountProvider(battle.A),
            battle.A.ArchEnemy != null && ReferenceEquals(battle.B, battle.A.ArchEnemy));

        var bScore = PowerCalculator.ApplyFanAndArchEnemyBonuses(
            PowerCalculator.CalculateGuardianCombatScore(battle.B),
            _fanCountProvider(battle.B),
            battle.B.ArchEnemy != null && ReferenceEquals(battle.A, battle.B.ArchEnemy));

        var winner = WeightedRandomChoice((battle.A, aScore), (battle.B, bScore));
        return winner;
    }

    /// <summary>
    /// Bepaal de winnaar van een teamgevecht. Telt de leden-scores op en kiest gewogen.
    /// </summary>
    public Team CalculateWinner(TeamBattle battle)
    {
        double aScore = 0;
        foreach (var m in battle.TeamA.Members)
        {
            var baseScore = PowerCalculator.CalculateGuardianCombatScore(m);
            aScore += PowerCalculator.ApplyFanAndArchEnemyBonuses(baseScore, _fanCountProvider(m), false);
        }

        double bScore = 0;
        foreach (var m in battle.TeamB.Members)
        {
            var baseScore = PowerCalculator.CalculateGuardianCombatScore(m);
            bScore += PowerCalculator.ApplyFanAndArchEnemyBonuses(baseScore, _fanCountProvider(m), false);
        }

        var winner = WeightedRandomChoice((battle.TeamA, aScore), (battle.TeamB, bScore));
        return winner;
    }

    public IndividualBattle CreateIndividualBattle(int id, DateTime at, string venue, Guardian a, Guardian b)
    {
        var winner = CalculateWinner(new Codex.Domain.Battles.IndividualBattle(id, at, venue, a, b, a)); // temp winner check
        // Kies opnieuw via losse berekening zonder bestaande Winner-constraint
        var aScore = PowerCalculator.ApplyFanAndArchEnemyBonuses(
            PowerCalculator.CalculateGuardianCombatScore(a), _fanCountProvider(a), a.ArchEnemy != null && ReferenceEquals(b, a.ArchEnemy));
        var bScore = PowerCalculator.ApplyFanAndArchEnemyBonuses(
            PowerCalculator.CalculateGuardianCombatScore(b), _fanCountProvider(b), b.ArchEnemy != null && ReferenceEquals(a, b.ArchEnemy));
        var finalWinner = WeightedRandomChoice((a, aScore), (b, bScore));
        return new Codex.Domain.Battles.IndividualBattle(id, at, venue, a, b, finalWinner);
    }

    public TeamBattle CreateTeamBattle(int id, DateTime at, string venue, Team teamA, Team teamB)
    {
        double aScore = 0, bScore = 0;
        foreach (var m in teamA.Members) aScore += PowerCalculator.ApplyFanAndArchEnemyBonuses(PowerCalculator.CalculateGuardianCombatScore(m), _fanCountProvider(m), false);
        foreach (var m in teamB.Members) bScore += PowerCalculator.ApplyFanAndArchEnemyBonuses(PowerCalculator.CalculateGuardianCombatScore(m), _fanCountProvider(m), false);
        var finalWinner = WeightedRandomChoice((teamA, aScore), (teamB, bScore));
        return new TeamBattle(id, at, venue, teamA, teamB, finalWinner);
    }

    /// <summary>
    /// Interne helper voor gewogen random keuze. Virtueel gemaakt voor deterministische tests.
    /// </summary>
    protected virtual T WeightedRandomChoice<T>((T item, double weight) a, (T item, double weight) b)
    {
        var aWeight = Math.Max(0.001, a.weight);
        var bWeight = Math.Max(0.001, b.weight);
        var total = aWeight + bWeight;
        var r = _random.NextDouble() * total;
        return r < aWeight ? a.item : b.item;
    }
}


