using Codex.Domain;
using Codex.Domain.Collections;
using Codex.Domain.Guardians;

namespace Codex.Tests;

public class SpellbookTests
{
    [Fact]
    public void Spellbook_Constructor_ValidatesName()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act & Assert: lege naam moet fout geven
        Assert.Throws<ArgumentException>(() => new Spellbook(1, "", "Description", mage, 100));
        Assert.Throws<ArgumentException>(() => new Spellbook(1, "   ", "Description", mage, 100));
    }

    [Fact]
    public void Spellbook_Constructor_ValidatesDescription()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act & Assert: lege beschrijving moet fout geven
        Assert.Throws<ArgumentException>(() => new Spellbook(1, "Name", "", mage, 100));
        Assert.Throws<ArgumentException>(() => new Spellbook(1, "Name", "   ", mage, 100));
    }

    [Fact]
    public void Spellbook_Constructor_ValidatesPages()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act & Assert: negatief aantal pagina's moet fout geven
        Assert.Throws<ArgumentOutOfRangeException>(() => new Spellbook(1, "Name", "Desc", mage, -1));
    }

    [Fact]
    public void Spellbook_Constructor_RequiresOwner()
    {
        // Act & Assert: null eigenaar moet fout geven
        Assert.Throws<ArgumentNullException>(() => new Spellbook(1, "Name", "Desc", null!, 100));
    }

    [Fact]
    public void Spellbook_Constructor_InitializesProperties()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act
        var spellbook = new Spellbook(100, "Codex Ignis", "Fire magic treatise", mage, 320);
        
        // Assert
        Assert.Equal(100, spellbook.Id);
        Assert.Equal("Codex Ignis", spellbook.Name);
        Assert.Equal("Fire magic treatise", spellbook.Description);
        Assert.Same(mage, spellbook.Owner);
        Assert.Equal(CollectionType.Spellbook, spellbook.Type);
        Assert.Equal(320, spellbook.Pages);
    }

    [Fact]
    public void Spellbook_Constructor_AutomaticallyAddsToOwner()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act
        var spellbook = new Spellbook(100, "Codex Ignis", "Fire magic", mage, 320);
        
        // Assert: eigenaar moet deze collectie hebben
        Assert.Contains(spellbook, mage.Collections);
    }

    [Fact]
    public void Spellbook_CanHaveZeroPages()
    {
        // Arrange
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        
        // Act: leeg spellbook met 0 pagina's
        var spellbook = new Spellbook(100, "Empty Book", "Blank pages", mage, 0);
        
        // Assert: moet goed werken
        Assert.Equal(0, spellbook.Pages);
    }
}

public class PaperArchiveTests
{
    [Fact]
    public void PaperArchive_Constructor_ValidatesName()
    {
        // Arrange
        var archivist = new Archivist(1, "Quorin", "Location", 50, null, 1000);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PaperArchive(1, "", "Description", archivist, 10));
    }

    [Fact]
    public void PaperArchive_Constructor_ValidatesDescription()
    {
        // Arrange
        var archivist = new Archivist(1, "Quorin", "Location", 50, null, 1000);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PaperArchive(1, "Name", "", archivist, 10));
    }

    [Fact]
    public void PaperArchive_Constructor_ValidatesShelves()
    {
        // Arrange
        var archivist = new Archivist(1, "Quorin", "Location", 50, null, 1000);
        
        // Act & Assert: negatief aantal planken moet fout geven
        Assert.Throws<ArgumentOutOfRangeException>(() => new PaperArchive(1, "Name", "Desc", archivist, -1));
    }

    [Fact]
    public void PaperArchive_Constructor_RequiresOwner()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PaperArchive(1, "Name", "Desc", null!, 10));
    }

    [Fact]
    public void PaperArchive_Constructor_InitializesProperties()
    {
        // Arrange
        var archivist = new Archivist(1, "Quorin", "Location", 50, null, 1000);
        
        // Act
        var archive = new PaperArchive(200, "Annals of Order", "Indexed records", archivist, 120);
        
        // Assert
        Assert.Equal(200, archive.Id);
        Assert.Equal("Annals of Order", archive.Name);
        Assert.Equal("Indexed records", archive.Description);
        Assert.Same(archivist, archive.Owner);
        Assert.Equal(CollectionType.PaperArchive, archive.Type);
        Assert.Equal(120, archive.Shelves);
    }

    [Fact]
    public void PaperArchive_Constructor_AutomaticallyAddsToOwner()
    {
        // Arrange
        var archivist = new Archivist(1, "Quorin", "Location", 50, null, 1000);
        
        // Act
        var archive = new PaperArchive(200, "Annals", "Records", archivist, 120);
        
        // Assert
        Assert.Contains(archive, archivist.Collections);
    }

    [Fact]
    public void PaperArchive_CanHaveZeroShelves()
    {
        // Arrange
        var archivist = new Archivist(1, "Quorin", "Location", 50, null, 1000);
        
        // Act
        var archive = new PaperArchive(200, "Empty Archive", "No shelves yet", archivist, 0);
        
        // Assert
        Assert.Equal(0, archive.Shelves);
    }
}

public class ServerClusterTests
{
    [Fact]
    public void ServerCluster_Constructor_ValidatesName()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ServerCluster(1, "", "Description", tech, 64, 256));
    }

    [Fact]
    public void ServerCluster_Constructor_ValidatesDescription()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ServerCluster(1, "Name", "", tech, 64, 256));
    }

    [Fact]
    public void ServerCluster_Constructor_ValidatesCpuCores()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act & Assert: negatief aantal CPU cores moet fout geven
        Assert.Throws<ArgumentOutOfRangeException>(() => new ServerCluster(1, "Name", "Desc", tech, -1, 256));
    }

    [Fact]
    public void ServerCluster_Constructor_ValidatesRamGb()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act & Assert: negatief RAM moet fout geven
        Assert.Throws<ArgumentOutOfRangeException>(() => new ServerCluster(1, "Name", "Desc", tech, 64, -1));
    }

    [Fact]
    public void ServerCluster_Constructor_RequiresOwner()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ServerCluster(1, "Name", "Desc", null!, 64, 256));
    }

    [Fact]
    public void ServerCluster_Constructor_InitializesProperties()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act
        var cluster = new ServerCluster(300, "Neon-1", "Primary compute", tech, 64, 256);
        
        // Assert
        Assert.Equal(300, cluster.Id);
        Assert.Equal("Neon-1", cluster.Name);
        Assert.Equal("Primary compute", cluster.Description);
        Assert.Same(tech, cluster.Owner);
        Assert.Equal(CollectionType.ServerCluster, cluster.Type);
        Assert.Equal(64, cluster.CpuCores);
        Assert.Equal(256, cluster.RamGb);
    }

    [Fact]
    public void ServerCluster_Constructor_AutomaticallyAddsToOwner()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act
        var cluster = new ServerCluster(300, "Neon-1", "Primary compute", tech, 64, 256);
        
        // Assert
        Assert.Contains(cluster, tech.Collections);
    }

    [Fact]
    public void ServerCluster_CanHaveZeroResources()
    {
        // Arrange
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act: cluster zonder resources
        var cluster = new ServerCluster(300, "Offline", "Inactive cluster", tech, 0, 0);
        
        // Assert
        Assert.Equal(0, cluster.CpuCores);
        Assert.Equal(0, cluster.RamGb);
    }
}

public class CollectionOwnershipTests
{
    [Fact]
    public void Collection_CanBeOwnedByDifferentGuardianType()
    {
        // Arrange: Technomancer kan een Spellbook bezitten (niet hun natuurlijke type)
        var tech = new Technomancer(1, "Vex", "Location", 50, null);
        
        // Act: creëer niet-natuurlijke collectie
        var spellbook = new Spellbook(100, "Codex Umbra", "Stolen book", tech, 210);
        
        // Assert: moet werken, krijgt alleen geen natuurlijke collectiebonus
        Assert.Same(tech, spellbook.Owner);
        Assert.Contains(spellbook, tech.Collections);
        Assert.Equal(CollectionType.Spellbook, spellbook.Type);
    }

    [Fact]
    public void Collection_TransferChangesOwner()
    {
        // Arrange: mage bezit een spellbook
        var mage = new Mage(1, "Aelith", "Location", 50, null, 100);
        var tech = new Technomancer(2, "Vex", "Location", 50, null);
        var spellbook = new Spellbook(100, "Book", "Desc", mage, 100);
        
        // Act: overdracht via guardian methode
        mage.TransferCollectionTo(spellbook, tech);
        
        // Assert: tech is nu eigenaar
        Assert.Same(tech, spellbook.Owner);
    }
}

