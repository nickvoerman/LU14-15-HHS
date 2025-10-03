using Codex.Domain.Battles;
using Codex.Domain.Collections;
using Codex.Domain.Guardians;
using Codex.Services;

namespace Codex.Tests;

// Deze subclass schakelt randomness uit door altijd het hoogste gewicht te kiezen
public class DeterministicBattleService : BattleService
{
    public DeterministicBattleService(Func<Guardian, int> fanCountProvider) : base(fanCountProvider, new Random(0)) { }
    protected override T WeightedRandomChoice<T>((T item, double weight) a, (T item, double weight) b)
    {
        return a.weight >= b.weight ? a.item : b.item;
    }
}

public class BattleServiceTests
{
    [Fact]
    public void HigherScoreGuardianWins_Deterministic()
    {
        // Arrange: Mage iets sterker dan Technomancer
        var mage = new Mage(1, "A", "L", 60, null, 100);
        mage.AddCollection(new Spellbook(1, "s", "d", mage, 10));
        var tech = new Technomancer(2, "B", "L", 40, null);
        var service = new DeterministicBattleService(_ => 0);
        var battle = service.CreateIndividualBattle(1, DateTime.Now, "Arena", mage, tech);

        // Act
        var winner = battle.Winner;

        // Assert
        Assert.Same(mage, winner);
    }

    [Fact]
    public void TeamBattle_RespectsTeamTypeAndMemberScores()
    {
        // Arrange: Mage-team heeft hogere somscore dan Tech-team
        var mage1 = new Mage(1, "A", "L", 60, null, 100);
        var mage2 = new Mage(2, "B", "L", 55, null, 80);
        var teamM = new Codex.Domain.Teams.Team(10, "Mages", new[] { mage1, mage2 });

        var t1 = new Technomancer(3, "C", "L", 50, null);
        var t2 = new Technomancer(4, "D", "L", 45, null);
        var teamT = new Codex.Domain.Teams.Team(11, "Techs", new[] { t1, t2 });

        var service = new DeterministicBattleService(_ => 0);
        var battle = service.CreateTeamBattle(2, DateTime.Now, "Arena", teamM, teamT);

        // Act
        var winner = battle.Winner;

        // Assert
        Assert.Same(teamM, winner);
    }
}


