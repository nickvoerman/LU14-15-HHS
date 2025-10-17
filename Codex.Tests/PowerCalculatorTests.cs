using Codex.Domain.Collections;
using Codex.Domain.Guardians;
using Codex.Domain.Teams;
using Codex.Services;

namespace Codex.Tests;

public class PowerCalculatorTests
{
    [Fact]
    public void CalculateGuardianCombatScore_BasedOnPowerLevel()
    {
        // Arrange: guardian with no collections
        var mage = new Mage(1, "Aelith", "Location", 80, null, 100);
        
        // Act
        var score = PowerCalculator.CalculateGuardianCombatScore(mage);
        
        // Assert: should equal power level
        Assert.Equal(80.0, score);
    }

    [Fact]
    public void CalculateGuardianCombatScore_CollectionsAddBonus()
    {
        // Arrange: mage with non-natural collection
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        var serverCluster = new ServerCluster(1, "Server", "Desc", tech, 64, 256);
        
        // Transfer non-natural collection to mage
        tech.TransferCollectionTo(serverCluster, mage);
        
        // Act
        var score = PowerCalculator.CalculateGuardianCombatScore(mage);
        
        // Assert: 50 (power) + 1.0 (1 collection) = 51.0
        Assert.Equal(51.0, score);
    }

    [Fact]
    public void CalculateGuardianCombatScore_NaturalCollectionsAddExtraBonus()
    {
        // Arrange: mage with natural collection (Spellbook)
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var spellbook = new Spellbook(1, "Codex", "Desc", mage, 100);
        
        // Act
        var score = PowerCalculator.CalculateGuardianCombatScore(mage);
        
        // Assert: 50 (power) + 1.0 (1 collection) + 1.5 (1 natural collection) = 52.5
        Assert.Equal(52.5, score);
    }

    [Fact]
    public void CalculateGuardianCombatScore_MultipleNaturalCollections()
    {
        // Arrange: mage with two natural collections
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var spellbook1 = new Spellbook(1, "Codex 1", "Desc", mage, 100);
        var spellbook2 = new Spellbook(2, "Codex 2", "Desc", mage, 50);
        
        // Act
        var score = PowerCalculator.CalculateGuardianCombatScore(mage);
        
        // Assert: 50 + (2 * 1.0) + (2 * 1.5) = 50 + 2 + 3 = 55.0
        Assert.Equal(55.0, score);
    }

    [Fact]
    public void CalculateGuardianCombatScore_MixedCollections()
    {
        // Arrange: mage with 1 natural and 1 non-natural collection
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        var spellbook = new Spellbook(1, "Codex", "Desc", mage, 100);
        var serverCluster = new ServerCluster(2, "Server", "Desc", tech, 64, 256);
        tech.TransferCollectionTo(serverCluster, mage);
        
        // Act
        var score = PowerCalculator.CalculateGuardianCombatScore(mage);
        
        // Assert: 50 + (2 * 1.0) [total collections] + (1 * 1.5) [natural] = 53.5
        Assert.Equal(53.5, score);
    }

    [Fact]
    public void CalculateGuardianCombatScore_TechnomancerNaturalType()
    {
        // Arrange: technomancer with natural ServerCluster
        var tech = new Technomancer(1, "Vex", "Location", 60, null);
        var cluster = new ServerCluster(1, "Server", "Desc", tech, 64, 256);
        
        // Act
        var score = PowerCalculator.CalculateGuardianCombatScore(tech);
        
        // Assert: 60 + 1.0 + 1.5 = 62.5
        Assert.Equal(62.5, score);
    }

    [Fact]
    public void CalculateGuardianCombatScore_ArchivistNaturalType()
    {
        // Arrange: archivist with natural PaperArchive
        var archivist = new Archivist(1, "Quorin", "Location", 70, null, 1000);
        var archive = new PaperArchive(1, "Archive", "Desc", archivist, 120);
        
        // Act
        var score = PowerCalculator.CalculateGuardianCombatScore(archivist);
        
        // Assert: 70 + 1.0 + 1.5 = 72.5
        Assert.Equal(72.5, score);
    }

    [Fact]
    public void ApplyFanAndArchEnemyBonuses_WithNoFans()
    {
        // Act: 50 base score, 0 fans, not fighting arch-enemy
        var score = PowerCalculator.ApplyFanAndArchEnemyBonuses(50.0, 0, false);
        
        // Assert: sqrt(0) = 0, so score = 50
        Assert.Equal(50.0, score);
    }

    [Fact]
    public void ApplyFanAndArchEnemyBonuses_WithFans()
    {
        // Act: 50 base score, 9 fans, not fighting arch-enemy
        var score = PowerCalculator.ApplyFanAndArchEnemyBonuses(50.0, 9, false);
        
        // Assert: sqrt(9) = 3, so score = 53
        Assert.Equal(53.0, score);
    }

    [Fact]
    public void ApplyFanAndArchEnemyBonuses_WithArchEnemyBoost()
    {
        // Act: 50 base score, 0 fans, fighting arch-enemy
        var score = PowerCalculator.ApplyFanAndArchEnemyBonuses(50.0, 0, true);
        
        // Assert: 50 * 1.1 = 55 (with tolerance for floating point)
        Assert.Equal(55.0, score, precision: 10);
    }

    [Fact]
    public void ApplyFanAndArchEnemyBonuses_WithFansAndArchEnemy()
    {
        // Act: 50 base score, 16 fans, fighting arch-enemy
        var score = PowerCalculator.ApplyFanAndArchEnemyBonuses(50.0, 16, true);
        
        // Assert: (50 + sqrt(16)) * 1.1 = (50 + 4) * 1.1 = 59.4 (with tolerance for floating point)
        Assert.Equal(59.4, score, precision: 10);
    }

    [Fact]
    public void ApplyFanAndArchEnemyBonuses_HandlesNegativeFanCount()
    {
        // Act: negative fan count should be treated as 0
        var score = PowerCalculator.ApplyFanAndArchEnemyBonuses(50.0, -5, false);
        
        // Assert: sqrt(max(0, -5)) = sqrt(0) = 0, score = 50
        Assert.Equal(50.0, score);
    }

    [Fact]
    public void CalculateTeamCombatScore_SumsAllMembers()
    {
        // Arrange: team with 2 members
        var tech1 = new Technomancer(1, "Vex", "Location", 50, null);
        var tech2 = new Technomancer(2, "Nyx", "Location", 60, null);
        var team = new Team(1, "Team", new[] { tech1, tech2 });
        
        // Act: no fans, no arch-enemy
        var score = PowerCalculator.CalculateTeamCombatScore(
            team,
            _ => 0,  // no fans
            (g, e) => false  // not arch-enemy
        );
        
        // Assert: 50 + 60 = 110
        Assert.Equal(110.0, score);
    }

    [Fact]
    public void CalculateTeamCombatScore_IncludesFanBonuses()
    {
        // Arrange: team with 2 members, first has 4 fans
        var tech1 = new Technomancer(1, "Vex", "Location", 50, null);
        var tech2 = new Technomancer(2, "Nyx", "Location", 60, null);
        var team = new Team(1, "Team", new[] { tech1, tech2 });
        
        // Act: tech1 has 4 fans
        var score = PowerCalculator.CalculateTeamCombatScore(
            team,
            g => g.Id == 1 ? 4 : 0,  // tech1 has 4 fans
            (g, e) => false
        );
        
        // Assert: (50 + sqrt(4)) + (60 + sqrt(0)) = 52 + 60 = 112
        Assert.Equal(112.0, score);
    }

    [Fact]
    public void CalculateTeamCombatScore_IncludesCollectionBonuses()
    {
        // Arrange: team with members that have collections
        var tech1 = new Technomancer(1, "Vex", "Location", 50, null);
        var tech2 = new Technomancer(2, "Nyx", "Location", 60, null);
        var cluster = new ServerCluster(1, "Server", "Desc", tech1, 64, 256);
        var team = new Team(1, "Team", new[] { tech1, tech2 });
        
        // Act: no fans
        var score = PowerCalculator.CalculateTeamCombatScore(
            team,
            _ => 0,
            (g, e) => false
        );
        
        // Assert: (50 + 1.0 + 1.5) + 60 = 52.5 + 60 = 112.5
        Assert.Equal(112.5, score);
    }
}



