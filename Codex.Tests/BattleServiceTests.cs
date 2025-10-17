using Codex.Domain.Battles;
using Codex.Domain.Collections;
using Codex.Domain.Guardians;
using Codex.Domain.Teams;
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

public class IndividualBattleTests
{
    [Fact]
    public void IndividualBattle_RequiresDifferentTypes()
    {
        // Arrange: two mages (same type)
        var mage1 = new Mage(1, "A", "L", 60, null, 100);
        var mage2 = new Mage(2, "B", "L", 60, null, 100);
        
        // Act & Assert: same type should throw
        Assert.Throws<InvalidOperationException>(() => 
            new IndividualBattle(1, DateTime.Now, "Arena", mage1, mage2, mage1));
    }

    [Fact]
    public void IndividualBattle_WinnerMustBeParticipant()
    {
        // Arrange: valid participants but wrong winner
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var outsider = new Archivist(3, "C", "L", 50, null, 1000);
        
        // Act & Assert: outsider cannot be winner
        Assert.Throws<ArgumentException>(() => 
            new IndividualBattle(1, DateTime.Now, "Arena", mage, tech, outsider));
    }

    [Fact]
    public void IndividualBattle_LoserIsOtherParticipant()
    {
        // Arrange
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        
        // Act: mage wins
        var battle = new IndividualBattle(1, DateTime.Now, "Arena", mage, tech, mage);
        
        // Assert
        Assert.Same(mage, battle.Winner);
        Assert.Same(tech, battle.Loser);
    }

    [Fact]
    public void IndividualBattle_InitializesProperties()
    {
        // Arrange
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var dateTime = new DateTime(2025, 10, 17, 14, 30, 0);
        
        // Act
        var battle = new IndividualBattle(10, dateTime, "Grand Arena", mage, tech, tech);
        
        // Assert
        Assert.Equal(10, battle.Id);
        Assert.Equal(dateTime, battle.DateTime);
        Assert.Equal("Grand Arena", battle.Venue);
        Assert.Same(mage, battle.A);
        Assert.Same(tech, battle.B);
        Assert.Same(tech, battle.Winner);
        Assert.Same(mage, battle.Loser);
    }
}

public class TeamBattleTests
{
    [Fact]
    public void TeamBattle_RequiresValidTeams()
    {
        // Arrange: team with only 1 member (invalid)
        var team1 = new Team(1, "Team1");
        team1.AddMember(new Technomancer(1, "A", "L", 50, null));
        
        var tech1 = new Technomancer(2, "B", "L", 50, null);
        var tech2 = new Technomancer(3, "C", "L", 50, null);
        var team2 = new Team(2, "Team2", new[] { tech1, tech2 });
        
        // Act & Assert: invalid team should throw
        Assert.Throws<InvalidOperationException>(() => 
            new TeamBattle(1, DateTime.Now, "Arena", team1, team2, team2));
    }

    [Fact]
    public void TeamBattle_RequiresDifferentTypes()
    {
        // Arrange: both teams are technomancers
        var tech1 = new Technomancer(1, "A", "L", 50, null);
        var tech2 = new Technomancer(2, "B", "L", 50, null);
        var team1 = new Team(1, "Team1", new[] { tech1, tech2 });
        
        var tech3 = new Technomancer(3, "C", "L", 50, null);
        var tech4 = new Technomancer(4, "D", "L", 50, null);
        var team2 = new Team(2, "Team2", new[] { tech3, tech4 });
        
        // Act & Assert: same type should throw
        Assert.Throws<InvalidOperationException>(() => 
            new TeamBattle(1, DateTime.Now, "Arena", team1, team2, team1));
    }

    [Fact]
    public void TeamBattle_WinnerMustBeParticipant()
    {
        // Arrange: valid teams but wrong winner
        var tech1 = new Technomancer(1, "A", "L", 50, null);
        var tech2 = new Technomancer(2, "B", "L", 50, null);
        var teamTech = new Team(1, "Techs", new[] { tech1, tech2 });
        
        var mage1 = new Mage(3, "C", "L", 50, null, 100);
        var mage2 = new Mage(4, "D", "L", 50, null, 100);
        var teamMage = new Team(2, "Mages", new[] { mage1, mage2 });
        
        var arch1 = new Archivist(5, "E", "L", 50, null, 1000);
        var arch2 = new Archivist(6, "F", "L", 50, null, 1000);
        var teamOutsider = new Team(3, "Outsiders", new[] { arch1, arch2 });
        
        // Act & Assert: outsider team cannot be winner
        Assert.Throws<ArgumentException>(() => 
            new TeamBattle(1, DateTime.Now, "Arena", teamTech, teamMage, teamOutsider));
    }

    [Fact]
    public void TeamBattle_InitializesProperties()
    {
        // Arrange
        var tech1 = new Technomancer(1, "A", "L", 50, null);
        var tech2 = new Technomancer(2, "B", "L", 50, null);
        var teamTech = new Team(1, "Techs", new[] { tech1, tech2 });
        
        var mage1 = new Mage(3, "C", "L", 50, null, 100);
        var mage2 = new Mage(4, "D", "L", 50, null, 100);
        var teamMage = new Team(2, "Mages", new[] { mage1, mage2 });
        
        var dateTime = new DateTime(2025, 10, 17, 14, 30, 0);
        
        // Act
        var battle = new TeamBattle(10, dateTime, "Grand Arena", teamTech, teamMage, teamMage);
        
        // Assert
        Assert.Equal(10, battle.Id);
        Assert.Equal(dateTime, battle.DateTime);
        Assert.Equal("Grand Arena", battle.Venue);
        Assert.Same(teamTech, battle.TeamA);
        Assert.Same(teamMage, battle.TeamB);
        Assert.Same(teamMage, battle.Winner);
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
    public void IndividualBattle_FansInfluenceOutcome()
    {
        // Arrange: weaker guardian with more fans
        var mage = new Mage(1, "A", "L", 50, null, 100);
        var tech = new Technomancer(2, "B", "L", 60, null); // stronger power level
        
        // Act: mage has 100 fans (sqrt(100) = 10), giving mage total 60 vs tech's 60
        var service = new DeterministicBattleService(g => g.Id == 1 ? 100 : 0);
        var battle = service.CreateIndividualBattle(1, DateTime.Now, "Arena", mage, tech);

        // Assert: with fans, mage should win (tie goes to first)
        Assert.Same(mage, battle.Winner);
    }

    [Fact]
    public void IndividualBattle_ArchEnemyBoostApplied()
    {
        // Arrange: slightly weaker guardian fighting their arch-enemy
        var mage = new Mage(1, "A", "L", 50, null, 100);
        var tech = new Technomancer(2, "B", "L", 54, null); // tech slightly stronger
        mage.SetArchEnemy(tech); // mage gets 10% boost
        
        // Act: mage gets arch-enemy boost: 50 * 1.1 = 55 > 54
        var service = new DeterministicBattleService(_ => 0);
        var battle = service.CreateIndividualBattle(1, DateTime.Now, "Arena", mage, tech);

        // Assert: mage should win with arch-enemy boost
        Assert.Same(mage, battle.Winner);
    }

    [Fact]
    public void IndividualBattle_NaturalCollectionBoostApplied()
    {
        // Arrange: guardians with different collection types
        var mage = new Mage(1, "A", "L", 50, null, 100);
        var tech = new Technomancer(2, "B", "L", 50, null);
        var spellbook = new Spellbook(1, "Book", "Desc", mage, 100); // natural for mage
        
        // Act: mage gets collection bonus
        var service = new DeterministicBattleService(_ => 0);
        var battle = service.CreateIndividualBattle(1, DateTime.Now, "Arena", mage, tech);

        // Assert: mage should win (52.5 vs 50)
        Assert.Same(mage, battle.Winner);
    }

    [Fact]
    public void TeamBattle_RespectsTeamTypeAndMemberScores()
    {
        // Arrange: Mage-team heeft hogere somscore dan Tech-team
        var mage1 = new Mage(1, "A", "L", 60, null, 100);
        var mage2 = new Mage(2, "B", "L", 55, null, 80);
        var teamM = new Team(10, "Mages", new[] { mage1, mage2 });

        var t1 = new Technomancer(3, "C", "L", 50, null);
        var t2 = new Technomancer(4, "D", "L", 45, null);
        var teamT = new Team(11, "Techs", new[] { t1, t2 });

        var service = new DeterministicBattleService(_ => 0);
        var battle = service.CreateTeamBattle(2, DateTime.Now, "Arena", teamM, teamT);

        // Act
        var winner = battle.Winner;

        // Assert
        Assert.Same(teamM, winner);
    }

    [Fact]
    public void TeamBattle_IncludesIndividualFanCounts()
    {
        // Arrange: tech team has fans
        var tech1 = new Technomancer(1, "A", "L", 50, null);
        var tech2 = new Technomancer(2, "B", "L", 50, null);
        var teamTech = new Team(1, "Techs", new[] { tech1, tech2 });
        
        var mage1 = new Mage(3, "C", "L", 50, null, 100);
        var mage2 = new Mage(4, "D", "L", 50, null, 100);
        var teamMage = new Team(2, "Mages", new[] { mage1, mage2 });
        
        // Act: tech1 has 16 fans (sqrt(16) = 4), giving techs edge
        var service = new DeterministicBattleService(g => g.Id == 1 ? 16 : 0);
        var battle = service.CreateTeamBattle(1, DateTime.Now, "Arena", teamTech, teamMage);

        // Assert: tech team should win (104 vs 100)
        Assert.Same(teamTech, battle.Winner);
    }

    [Fact]
    public void TeamBattle_IncludesCollectionBonuses()
    {
        // Arrange: tech team has natural collections
        var tech1 = new Technomancer(1, "A", "L", 50, null);
        var tech2 = new Technomancer(2, "B", "L", 50, null);
        var cluster = new ServerCluster(1, "Server", "Desc", tech1, 64, 256);
        var teamTech = new Team(1, "Techs", new[] { tech1, tech2 });
        
        var mage1 = new Mage(3, "C", "L", 50, null, 100);
        var mage2 = new Mage(4, "D", "L", 50, null, 100);
        var teamMage = new Team(2, "Mages", new[] { mage1, mage2 });
        
        // Act: tech1 has natural collection (50 + 1.0 + 1.5 = 52.5)
        var service = new DeterministicBattleService(_ => 0);
        var battle = service.CreateTeamBattle(1, DateTime.Now, "Arena", teamTech, teamMage);

        // Assert: tech team should win (102.5 vs 100)
        Assert.Same(teamTech, battle.Winner);
    }

    [Fact]
    public void CalculateWinner_HandlesEqualScores()
    {
        // Arrange: two guardians with identical scores
        var mage = new Mage(1, "A", "L", 50, null, 100);
        var tech = new Technomancer(2, "B", "L", 50, null);
        
        // Act: deterministic service picks first on tie
        var service = new DeterministicBattleService(_ => 0);
        var battle = new IndividualBattle(1, DateTime.Now, "Arena", mage, tech, mage);
        var winner = service.CalculateWinner(battle);

        // Assert: should handle tie gracefully (first wins in deterministic)
        Assert.Same(mage, winner);
    }
}


