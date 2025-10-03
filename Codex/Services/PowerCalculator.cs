namespace Codex.Services;

using System;
using System.Linq;
using Codex.Domain;
using Codex.Domain.Collections;
using Codex.Domain.Guardians;
using Codex.Domain.Teams;

/// <summary>
/// Berekent gevechtsscores voor Guardians en Teams. Regels:
/// - Basis is het power level.
/// - Elke collectie voegt een klein beetje toe.
/// - Collecties van het natuurlijke type geven extra bonus.
/// - Fans en aartsvijand-bonus worden apart toegepast, zodat we dit los kunnen testen/injecteren.
/// </summary>
public static class PowerCalculator
{
    /// <summary>
    /// Bepaal de basisscore van een Guardian op basis van power en collecties (zonder fans/bonussen).
    /// </summary>
    public static double CalculateGuardianCombatScore(Guardian guardian)
    {
        double score = guardian.PowerLevel;

        int totalCollections = guardian.Collections.Count;
        int naturalCollections = guardian.Collections.Count(c => IsNaturalCollection(guardian, c));

        score += totalCollections * 1.0;          // elke collectie telt mee
        score += naturalCollections * 1.5;        // natuurlijke collecties tellen extra mee

        // Fans en arch-enemy bonussen worden elders toegepast
        return score;
    }

    /// <summary>
    /// Pas fans (wortelgroei om extreme aantallen te beperken) en aartsvijand-boost toe.
    /// </summary>
    public static double ApplyFanAndArchEnemyBonuses(double baseScore, int fanCount, bool fightingArchEnemy)
    {
        double score = baseScore + Math.Sqrt(Math.Max(0, fanCount));
        if (fightingArchEnemy) score *= 1.1; // kleine adrenalineboost tegen aartsvijand
        return score;
    }

    /// <summary>
    /// Teamscore is de som van de leden met toegepaste fanbonus per lid.
    /// </summary>
    public static double CalculateTeamCombatScore(Team team, Func<Guardian, int> fanCountProvider, Func<Guardian, Guardian?, bool> isArchEnemy)
    {
        double total = 0;
        foreach (var member in team.Members)
        {
            var baseScore = CalculateGuardianCombatScore(member);
            var score = ApplyFanAndArchEnemyBonuses(baseScore, fanCountProvider(member), false);
            total += score;
        }
        return total;
    }

    private static bool IsNaturalCollection(Guardian guardian, Collection collection)
    {
        return guardian switch
        {
            Mage => collection.Type == CollectionType.Spellbook,
            Archivist => collection.Type == CollectionType.PaperArchive,
            Technomancer => collection.Type == CollectionType.ServerCluster,
            _ => false
        };
    }
}


