using Codex.Domain.Fans;
using Codex.Domain.Guardians;
using Codex.Services;

namespace Codex.Tests;

public class FanServiceTests
{
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
}


