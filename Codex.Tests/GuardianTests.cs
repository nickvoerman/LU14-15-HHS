using Codex.Domain;
using Codex.Domain.Collections;
using Codex.Domain.Guardians;

namespace Codex.Tests;

public class GuardianTests
{
    [Fact]
    public void Guardian_Constructor_ValidatesName()
    {
        // Assert: empty name should throw
        Assert.Throws<ArgumentException>(() => new Mage(1, "", "Location", 50, null, 100));
        Assert.Throws<ArgumentException>(() => new Mage(1, "   ", "Location", 50, null, 100));
    }

    [Fact]
    public void Guardian_Constructor_ValidatesLocation()
    {
        // Assert: empty location should throw
        Assert.Throws<ArgumentException>(() => new Mage(1, "Name", "", 50, null, 100));
        Assert.Throws<ArgumentException>(() => new Mage(1, "Name", "   ", 50, null, 100));
    }

    [Fact]
    public void Guardian_Constructor_ValidatesPowerLevel()
    {
        // Assert: negative power level should throw
        Assert.Throws<ArgumentOutOfRangeException>(() => new Mage(1, "Name", "Location", -1, null, 100));
    }

    [Fact]
    public void Guardian_Constructor_UsesCatchphraseOrDefault()
    {
        // Arrange & Act: with custom catchphrase
        var mage1 = new Mage(1, "Name", "Location", 50, "Custom phrase", 100);
        
        // Assert
        Assert.Equal("Custom phrase", mage1.Catchphrase);

        // Arrange & Act: without catchphrase (should use default)
        var mage2 = new Mage(2, "Name", "Location", 50, null, 100);
        
        // Assert: standaard catchphrase
        Assert.Equal("Knowledge is power, and I wield it.", mage2.Catchphrase);
    }

    [Fact]
    public void AddEnemy_CannotAddSelf()
    {
        // Arrange
        var mage = new Mage(1, "Name", "Location", 50, null, 100);
        
        // Act & Assert: kan zichzelf niet als vijand toevoegen
        Assert.Throws<InvalidOperationException>(() => mage.AddEnemy(mage));
    }

    [Fact]
    public void AddEnemy_CannotAddSameType()
    {
        // Arrange: twee mages
        var mage1 = new Mage(1, "Aelith", "Location", 50, null, 100);
        var mage2 = new Mage(2, "Seren", "Location", 50, null, 100);
        
        // Act & Assert: kan geen vijand van hetzelfde type toevoegen
        Assert.Throws<InvalidOperationException>(() => mage1.AddEnemy(mage2));
    }

    [Fact]
    public void AddEnemy_CanAddDifferentType()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        
        // Act
        mage.AddEnemy(tech);
        
        // Assert
        Assert.Contains(tech, mage.Enemies);
    }

    [Fact]
    public void AddEnemy_IgnoresDuplicates()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        
        // Act: voeg dezelfde vijand twee keer toe
        mage.AddEnemy(tech);
        mage.AddEnemy(tech);
        
        // Assert: should only appear once
        Assert.Single(mage.Enemies);
    }

    [Fact]
    public void SetArchEnemy_CannotSetSelf()
    {
        // Arrange
        var mage = new Mage(1, "Name", "Location", 50, null, 100);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => mage.SetArchEnemy(mage));
    }

    [Fact]
    public void SetArchEnemy_CannotSetSameType()
    {
        // Arrange
        var mage1 = new Mage(1, "Aelith", "Location", 50, null, 100);
        var mage2 = new Mage(2, "Seren", "Location", 50, null, 100);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => mage1.SetArchEnemy(mage2));
    }

    [Fact]
    public void SetArchEnemy_AddsToEnemiesList()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        
        // Act: stel aartsvijand in (niet eerder in vijandlijst)
        mage.SetArchEnemy(tech);
        
        // Assert: moet in zowel ArchEnemy als Enemies zitten
        Assert.Same(tech, mage.ArchEnemy);
        Assert.Contains(tech, mage.Enemies);
    }

    [Fact]
    public void SetArchEnemy_WorksWhenAlreadyEnemy()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        mage.AddEnemy(tech); // voeg eerst toe als gewone vijand
        
        // Act: stel nu in als aartsvijand
        mage.SetArchEnemy(tech);
        
        // Assert: moet aartsvijand zijn en nog steeds maar één keer in vijandlijst
        Assert.Same(tech, mage.ArchEnemy);
        Assert.Single(mage.Enemies);
    }

    [Fact]
    public void AddCollection_RequiresMatchingOwner()
    {
        // Arrange: collectie behoort toe aan mage1
        var mage1 = new Mage(1, "Aelith", "Location", 50, null, 100);
        var mage2 = new Mage(2, "Seren", "Location", 50, null, 100);
        var spellbook = new Spellbook(100, "Book", "Desc", mage1, 100);
        
        // Act & Assert: mage2 kan mage1's collectie niet toevoegen
        Assert.Throws<InvalidOperationException>(() => mage2.AddCollection(spellbook));
    }

    [Fact]
    public void AddCollection_IgnoresDuplicates()
    {
        // Arrange: collectie al toegevoegd via constructor
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var spellbook = new Spellbook(100, "Book", "Desc", mage, 100);
        
        // Act: probeer opnieuw toe te voegen (al toegevoegd in Spellbook constructor)
        mage.AddCollection(spellbook);
        
        // Assert: should only appear once
        Assert.Single(mage.Collections);
    }

    [Fact]
    public void RemoveCollection_ThrowsInvalidOperation()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var spellbook = new Spellbook(100, "Book", "Desc", mage, 100);
        
        // Act & Assert: collecties kunnen niet verwijderd worden, alleen overgedragen
        Assert.Throws<InvalidOperationException>(() => mage.RemoveCollection(spellbook));
    }

    [Fact]
    public void TransferCollectionTo_MovesOwnership()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        var spellbook = new Spellbook(100, "Book", "Desc", mage, 100);
        
        // Act: draag over van mage naar tech
        mage.TransferCollectionTo(spellbook, tech);
        
        // Assert: tech bezit het nu, mage niet
        Assert.DoesNotContain(spellbook, mage.Collections);
        Assert.Contains(spellbook, tech.Collections);
        Assert.Same(tech, spellbook.Owner);
    }

    [Fact]
    public void TransferCollectionTo_FailsIfNotOwner()
    {
        // Arrange: mage1 bezit de collectie
        var mage1 = new Mage(1, "Aelith", "Location", 50, null, 100);
        var mage2 = new Mage(2, "Seren", "Location", 50, null, 100);
        var tech = new Technomancer(3, "Vex", "Location", 50, null);
        var spellbook = new Spellbook(100, "Book", "Desc", mage1, 100);
        
        // Act & Assert: mage2 kan mage1's collectie niet overdragen
        Assert.Throws<InvalidOperationException>(() => mage2.TransferCollectionTo(spellbook, tech));
    }

    [Fact]
    public void TransferCollectionTo_NoOpIfSameOwner()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var spellbook = new Spellbook(100, "Book", "Desc", mage, 100);
        
        // Act: draag over naar zelf (moet niets doen)
        mage.TransferCollectionTo(spellbook, mage);
        
        // Assert: bezit het nog steeds
        Assert.Contains(spellbook, mage.Collections);
        Assert.Same(mage, spellbook.Owner);
    }
}

public class MageTests
{
    [Fact]
    public void Mage_Constructor_ValidatesManaReserve()
    {
        // Assert: negatieve mana moet fout geven
        Assert.Throws<ArgumentOutOfRangeException>(() => new Mage(1, "Name", "Location", 50, null, -1));
    }

    [Fact]
    public void Mage_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var mage = new Mage(1, "Aelith", "Arcane Spire", 80, "By the runes!", 150);
        
        // Assert
        Assert.Equal(1, mage.Id);
        Assert.Equal("Aelith", mage.Name);
        Assert.Equal("Arcane Spire", mage.Location);
        Assert.Equal(80, mage.PowerLevel);
        Assert.Equal("By the runes!", mage.Catchphrase);
        Assert.Equal(150, mage.ManaReserve);
        Assert.Equal(GuardianType.Mage, mage.Type);
        Assert.Empty(mage.Spells);
    }

    [Fact]
    public void LearnSpell_AddsNewSpell()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act
        mage.LearnSpell("Fireball");
        mage.LearnSpell("Ice Bolt");
        
        // Assert
        Assert.Equal(2, mage.Spells.Count);
        Assert.Contains("Fireball", mage.Spells);
        Assert.Contains("Ice Bolt", mage.Spells);
    }

    [Fact]
    public void LearnSpell_IgnoresDuplicates()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act: leer dezelfde spreuk twee keer
        mage.LearnSpell("Fireball");
        mage.LearnSpell("Fireball");
        
        // Assert: should only appear once
        Assert.Single(mage.Spells);
    }

    [Fact]
    public void LearnSpell_IgnoresEmptyOrWhitespace()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act
        mage.LearnSpell("");
        mage.LearnSpell("   ");
        mage.LearnSpell(null!);
        
        // Assert: geen spreuken geleerd
        Assert.Empty(mage.Spells);
    }
}

public class TechnomancerTests
{
    [Fact]
    public void Technomancer_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var tech = new Technomancer(1, "Vex", "Neon Grid", 75, "Optimize!");
        
        // Assert
        Assert.Equal(1, tech.Id);
        Assert.Equal("Vex", tech.Name);
        Assert.Equal("Neon Grid", tech.Location);
        Assert.Equal(75, tech.PowerLevel);
        Assert.Equal("Optimize!", tech.Catchphrase);
        Assert.Equal(GuardianType.Technomancer, tech.Type);
        Assert.Empty(tech.Algorithms);
    }

    [Fact]
    public void AddAlgorithm_AddsNewAlgorithm()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act
        tech.AddAlgorithm("A*");
        tech.AddAlgorithm("Dijkstra");
        
        // Assert
        Assert.Equal(2, tech.Algorithms.Count);
        Assert.Contains("A*", tech.Algorithms);
        Assert.Contains("Dijkstra", tech.Algorithms);
    }

    [Fact]
    public void AddAlgorithm_IgnoresDuplicates()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act: voeg hetzelfde algoritme twee keer toe
        tech.AddAlgorithm("A*");
        tech.AddAlgorithm("A*");
        
        // Assert: should only appear once
        Assert.Single(tech.Algorithms);
    }

    [Fact]
    public void AddAlgorithm_IgnoresEmptyOrWhitespace()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act
        tech.AddAlgorithm("");
        tech.AddAlgorithm("   ");
        tech.AddAlgorithm(null!);
        
        // Assert: geen algoritmes toegevoegd
        Assert.Empty(tech.Algorithms);
    }
}

public class ArchivistTests
{
    [Fact]
    public void Archivist_Constructor_ValidatesDocumentCount()
    {
        // Assert: negatief aantal documenten moet fout geven
        Assert.Throws<ArgumentOutOfRangeException>(() => new Archivist(1, "Name", "Location", 50, null, -1));
    }

    [Fact]
    public void Archivist_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var archivist = new Archivist(1, "Quorin", "Grand Archive", 70, "Order prevails.", 5000);
        
        // Assert
        Assert.Equal(1, archivist.Id);
        Assert.Equal("Quorin", archivist.Name);
        Assert.Equal("Grand Archive", archivist.Location);
        Assert.Equal(70, archivist.PowerLevel);
        Assert.Equal("Order prevails.", archivist.Catchphrase);
        Assert.Equal(5000, archivist.DocumentCount);
        Assert.Equal(GuardianType.Archivist, archivist.Type);
    }
}

