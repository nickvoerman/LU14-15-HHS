using Codex.Domain.Fans;
using Codex.Domain.Guardians;

namespace Codex.Tests;

public class FanTests
{
    [Fact]
    public void Fan_Constructor_ValidatesName()
    {
        // Assert: empty name should throw
        Assert.Throws<ArgumentException>(() => new Fan(1, ""));
        Assert.Throws<ArgumentException>(() => new Fan(1, "   "));
        Assert.Throws<ArgumentException>(() => new Fan(1, null!));
    }

    [Fact]
    public void Fan_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var fan = new Fan(1, "Lina");
        
        // Assert
        Assert.Equal(1, fan.Id);
        Assert.Equal("Lina", fan.Name);
        Assert.Empty(fan.FavoriteGuardians);
    }

    [Fact]
    public void Follow_AddsGuardianToFavorites()
    {
        // Arrange
        var fan = new Fan(1, "Lina");
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act
        fan.Follow(mage);
        
        // Assert
        Assert.Contains(mage, fan.FavoriteGuardians);
    }

    [Fact]
    public void Follow_CanFollowMultipleGuardians()
    {
        // Arrange
        var fan = new Fan(1, "Lina");
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        var archivist = new Archivist(3, "Quorin", "Location", 50, null, 1000);
        
        // Act
        fan.Follow(mage);
        fan.Follow(tech);
        fan.Follow(archivist);
        
        // Assert
        Assert.Equal(3, fan.FavoriteGuardians.Count);
        Assert.Contains(mage, fan.FavoriteGuardians);
        Assert.Contains(tech, fan.FavoriteGuardians);
        Assert.Contains(archivist, fan.FavoriteGuardians);
    }

    [Fact]
    public void Follow_IgnoresDuplicates()
    {
        // Arrange
        var fan = new Fan(1, "Lina");
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act: follow same guardian twice
        fan.Follow(mage);
        fan.Follow(mage);
        
        // Assert: should only appear once (HashSet behavior)
        Assert.Single(fan.FavoriteGuardians);
    }

    [Fact]
    public void Follow_ThrowsOnNullGuardian()
    {
        // Arrange
        var fan = new Fan(1, "Lina");
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => fan.Follow(null!));
    }

    [Fact]
    public void Unfollow_RemovesGuardianFromFavorites()
    {
        // Arrange
        var fan = new Fan(1, "Lina");
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        fan.Follow(mage);
        
        // Act
        fan.Unfollow(mage);
        
        // Assert
        Assert.DoesNotContain(mage, fan.FavoriteGuardians);
    }

    [Fact]
    public void Unfollow_DoesNothingIfNotFollowing()
    {
        // Arrange
        var fan = new Fan(1, "Lina");
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act: unfollow without following first
        fan.Unfollow(mage);
        
        // Assert: no error, just empty favorites
        Assert.Empty(fan.FavoriteGuardians);
    }

    [Fact]
    public void Unfollow_ThrowsOnNullGuardian()
    {
        // Arrange
        var fan = new Fan(1, "Lina");
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => fan.Unfollow(null!));
    }

    [Fact]
    public void Unfollow_CanUnfollowOneOfMultiple()
    {
        // Arrange: fan follows two guardians
        var fan = new Fan(1, "Lina");
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        fan.Follow(mage);
        fan.Follow(tech);
        
        // Act: unfollow just the mage
        fan.Unfollow(mage);
        
        // Assert: still follows tech
        Assert.DoesNotContain(mage, fan.FavoriteGuardians);
        Assert.Contains(tech, fan.FavoriteGuardians);
        Assert.Single(fan.FavoriteGuardians);
    }
}

