using Codex.Domain.Fans;
using Codex.Domain.Guardians;
using Codex.Services;

namespace Codex.Tests;

public class FanServiceTests
{
    [Fact]
    public void CountFansOf_ReturnsCorrectCount()
    {
        // Arrange: 3 fans, 2 follow mage, 1 follows tech
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fans = new List<Fan>
        {
            new Fan(1, "f1"),
            new Fan(2, "f2"),
            new Fan(3, "f3")
        };
        fans[0].Follow(mage);
        fans[1].Follow(mage);
        fans[2].Follow(tech);
        var service = new FanService();

        // Act & Assert
        Assert.Equal(2, service.CountFansOf(mage, fans));
        Assert.Equal(1, service.CountFansOf(tech, fans));
    }

    [Fact]
    public void CountFansOf_ReturnsZeroWhenNoFans()
    {
        // Arrange: guardian with no fans
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var fans = new List<Fan> { new Fan(1, "f1"), new Fan(2, "f2") };
        var service = new FanService();

        // Act & Assert
        Assert.Equal(0, service.CountFansOf(mage, fans));
    }

    [Fact]
    public void CountFansOf_HandlesEmptyFanList()
    {
        // Arrange: no fans at all
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var fans = new List<Fan>();
        var service = new FanService();

        // Act & Assert
        Assert.Equal(0, service.CountFansOf(mage, fans));
    }

    [Fact]
    public void CountFansOf_HandlesFanFollowingMultipleGuardians()
    {
        // Arrange: fan follows both guardians
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fan = new Fan(1, "f1");
        fan.Follow(mage);
        fan.Follow(tech);
        var fans = new List<Fan> { fan };
        var service = new FanService();

        // Act & Assert: should count once per guardian
        Assert.Equal(1, service.CountFansOf(mage, fans));
        Assert.Equal(1, service.CountFansOf(tech, fans));
    }

    [Fact]
    public void CountFansOf_ThrowsOnNullGuardian()
    {
        // Arrange
        var fans = new List<Fan>();
        var service = new FanService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.CountFansOf(null!, fans));
    }

    [Fact]
    public void CountFansOf_ThrowsOnNullFanList()
    {
        // Arrange
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var service = new FanService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.CountFansOf(mage, null!));
    }

    [Fact]
    public void TransferFans_MovesRequestedCountWhenAvailable()
    {
        // Arrange: twee fans volgen de verliezer (Mage), één volgt de winnaar (Technomancer)
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fans = new List<Fan> { new Fan(1, "f1"), new Fan(2, "f2"), new Fan(3, "f3") };
        fans[0].Follow(mage);
        fans[1].Follow(mage);
        fans[2].Follow(tech);
        var service = new FanService();

        // Act: verplaats 1 fan van mage naar tech
        var moved = service.TransferFans(mage, tech, fans, 1);

        // Assert: controleer tellingen en aantal verplaatsten
        Assert.Equal(1, moved);
        Assert.Equal(1, service.CountFansOf(mage, fans));
        Assert.Equal(2, service.CountFansOf(tech, fans));
    }

    [Fact]
    public void TransferFans_MovesAllRequestedWhenAvailable()
    {
        // Arrange: 3 fans follow loser
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fans = new List<Fan>
        {
            new Fan(1, "f1"),
            new Fan(2, "f2"),
            new Fan(3, "f3")
        };
        fans[0].Follow(mage);
        fans[1].Follow(mage);
        fans[2].Follow(mage);
        var service = new FanService();

        // Act: transfer 2 fans
        var moved = service.TransferFans(mage, tech, fans, 2);

        // Assert
        Assert.Equal(2, moved);
        Assert.Equal(1, service.CountFansOf(mage, fans));
        Assert.Equal(2, service.CountFansOf(tech, fans));
    }

    [Fact]
    public void TransferFans_ReturnsActualCountWhenInsufficientFans()
    {
        // Arrange: only 2 fans follow loser, but we request 5
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fans = new List<Fan>
        {
            new Fan(1, "f1"),
            new Fan(2, "f2"),
            new Fan(3, "f3")
        };
        fans[0].Follow(mage);
        fans[1].Follow(mage);
        fans[2].Follow(tech); // doesn't follow mage
        var service = new FanService();

        // Act: request 5, but only 2 available
        var moved = service.TransferFans(mage, tech, fans, 5);

        // Assert: only 2 moved
        Assert.Equal(2, moved);
        Assert.Equal(0, service.CountFansOf(mage, fans));
        Assert.Equal(3, service.CountFansOf(tech, fans));
    }

    [Fact]
    public void TransferFans_ReturnsZeroWhenNoFansFollow()
    {
        // Arrange: no fans follow the loser
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fans = new List<Fan> { new Fan(1, "f1"), new Fan(2, "f2") };
        fans[0].Follow(tech);
        fans[1].Follow(tech);
        var service = new FanService();

        // Act: try to transfer from mage (who has no fans)
        var moved = service.TransferFans(mage, tech, fans, 1);

        // Assert: nothing moved
        Assert.Equal(0, moved);
        Assert.Equal(0, service.CountFansOf(mage, fans));
        Assert.Equal(2, service.CountFansOf(tech, fans));
    }

    [Fact]
    public void TransferFans_ReturnsZeroWhenCountIsZero()
    {
        // Arrange
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fans = new List<Fan> { new Fan(1, "f1") };
        fans[0].Follow(mage);
        var service = new FanService();

        // Act: transfer 0 fans
        var moved = service.TransferFans(mage, tech, fans, 0);

        // Assert: nothing changed
        Assert.Equal(0, moved);
        Assert.Equal(1, service.CountFansOf(mage, fans));
        Assert.Equal(0, service.CountFansOf(tech, fans));
    }

    [Fact]
    public void TransferFans_ReturnsZeroWhenCountIsNegative()
    {
        // Arrange
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fans = new List<Fan> { new Fan(1, "f1") };
        fans[0].Follow(mage);
        var service = new FanService();

        // Act: transfer negative count (should do nothing)
        var moved = service.TransferFans(mage, tech, fans, -5);

        // Assert: nothing changed
        Assert.Equal(0, moved);
        Assert.Equal(1, service.CountFansOf(mage, fans));
        Assert.Equal(0, service.CountFansOf(tech, fans));
    }

    [Fact]
    public void TransferFans_FanCanFollowBothAfterTransfer()
    {
        // Arrange: fan follows both initially
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fan1 = new Fan(1, "f1");
        fan1.Follow(mage);
        fan1.Follow(tech);
        var fan2 = new Fan(2, "f2");
        fan2.Follow(mage);
        var fans = new List<Fan> { fan1, fan2 };
        var service = new FanService();

        // Act: transfer 1 fan from mage to tech
        var moved = service.TransferFans(mage, tech, fans, 1);

        // Assert: fan1 was transferred (unfollowed mage, still follows tech once)
        Assert.Equal(1, moved);
        Assert.Equal(1, service.CountFansOf(mage, fans)); // fan2 still follows
        Assert.Equal(1, service.CountFansOf(tech, fans)); // fan1 follows tech (not duplicated)
    }

    [Fact]
    public void TransferFans_ThrowsOnNullFrom()
    {
        // Arrange
        var tech = new Technomancer(1, "A", "L", 40, null);
        var fans = new List<Fan>();
        var service = new FanService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.TransferFans(null!, tech, fans, 1));
    }

    [Fact]
    public void TransferFans_ThrowsOnNullTo()
    {
        // Arrange
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var fans = new List<Fan>();
        var service = new FanService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.TransferFans(mage, null!, fans, 1));
    }

    [Fact]
    public void TransferFans_ThrowsOnNullFanList()
    {
        // Arrange
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var service = new FanService();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.TransferFans(mage, tech, null!, 1));
    }

    [Fact]
    public void TransferFans_HandlesEmptyFanList()
    {
        // Arrange: no fans at all
        var mage = new Mage(1, "A", "L", 60, null, 100);
        var tech = new Technomancer(2, "B", "L", 40, null);
        var fans = new List<Fan>();
        var service = new FanService();

        // Act
        var moved = service.TransferFans(mage, tech, fans, 1);

        // Assert: nothing to transfer
        Assert.Equal(0, moved);
    }
}



