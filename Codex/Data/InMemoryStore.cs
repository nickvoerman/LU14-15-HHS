namespace Codex.Data;

using System;
using System.Collections.Generic;
using Codex.Domain.Collections;
using Codex.Domain.Fans;
using Codex.Domain.Guardians;
using Codex.Domain.Teams;

/// <summary>
/// Eenvoudige in-memory data-opslag voor de console app. Hier zaaien we (seed)
/// voorbeelddata zodat de applicatie direct iets kan tonen en testen.
/// </summary>
public sealed class InMemoryStore
{
    public List<Guardian> Guardians { get; } = new();
    public List<Fan> Fans { get; } = new();
    public List<Collection> Collections { get; } = new();
    public List<Team> Teams { get; } = new();

    /// <summary>
    /// Maak voorbeeld Guardians, Collecties, Fans en Teams aan en koppel ze aan elkaar.
    /// Regels die we terugzien:
    /// - Vijanden alleen tussen verschillende typen
    /// - Arch-enemy ingesteld en toegevoegd aan enemies
    /// - Collecties gekoppeld aan één eigenaar
    /// - Teams met leden van hetzelfde type en minimaal twee leden
    /// </summary>
    public void Seed()
    {
        var mage = new Mage(1, "Aelith", "Arcane Spire", 80, "By the runes, kneel!", 120);
        mage.LearnSpell("Fireball");
        mage.LearnSpell("Blink");

        var archivist = new Archivist(2, "Quorin", "Grand Archive", 70, "Order shall prevail.", 5000);
        var techno = new Technomancer(3, "Vex", "Neon Grid", 75, "Optimize and dominate.");
        techno.AddAlgorithm("AlphaFold");
        techno.AddAlgorithm("A* Pathfinding");

        mage.AddEnemy(archivist);
        mage.AddEnemy(techno);
        mage.SetArchEnemy(techno);

        archivist.AddEnemy(mage);
        archivist.AddEnemy(techno);
        archivist.SetArchEnemy(mage);

        techno.AddEnemy(mage);
        techno.AddEnemy(archivist);
        techno.SetArchEnemy(archivist);

        var sb1 = new Spellbook(100, "Codex Ignis", "Treatise on flame", mage, 320);
        var pa1 = new PaperArchive(200, "Annals of Order", "Indexed records", archivist, 120);
        var sc1 = new ServerCluster(300, "Neon-1", "Primary compute", techno, 64, 256);

        var sb2 = new Spellbook(101, "Codex Umbra", "Shadows and veils", techno, 210); // off-type

        var fan1 = new Fan(1, "Lina");
        fan1.Follow(mage);
        fan1.Follow(techno);

        var fan2 = new Fan(2, "Darius");
        fan2.Follow(archivist);

        Guardians.AddRange(new Guardian[] { mage, archivist, techno });
        Collections.AddRange(new Collection[] { sb1, pa1, sc1, sb2 });
        Fans.AddRange(new[] { fan1, fan2 });

        var teamTech = new Team(10, "Neon Syndicate", new[] { techno, new Technomancer(4, "Nyx", "Neon Grid", 68, null) });

        var teamMages = new Team(11, "Circle of Runes", new[] { mage, new Mage(6, "Seren", "Arcane Spire", 65, null, 80) });

        Teams.Add(teamTech);
        Teams.Add(teamMages);
    }
}


