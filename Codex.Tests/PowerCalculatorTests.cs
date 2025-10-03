using Codex.Domain.Collections;
using Codex.Domain.Guardians;
using Codex.Services;

namespace Codex.Tests;

public class PowerCalculatorTests
{
    [Fact]
    public void GuardianNaturalCollectionsBoostScore()
    {
        // Doel: natuurlijke collecties moeten extra bijdragen aan de basisscore
        var mage = new Mage(1, "Aelith", "Arcane", 50, null, 100);
        mage.AddCollection(new Spellbook(1, "SB", "beschrijving", mage, 100));
        mage.AddCollection(new Spellbook(2, "SB2", "beschrijving", mage, 50));
        var baseScore = PowerCalculator.CalculateGuardianCombatScore(mage);
        Assert.True(baseScore > 50 + 2.0); // 2 collecties -> > 50 + (2 * 1.0)
    }
}


