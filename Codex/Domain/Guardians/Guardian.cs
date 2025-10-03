namespace Codex.Domain.Guardians;

using System;
using System.Collections.Generic;
using Codex.Domain;
using Codex.Domain.Collections;

/// <summary>
/// Basisklasse voor alle Guardians (Mage, Archivist, Technomancer).
/// Bevat gedeelde eigenschappen zoals id, naam, locatie, power level en catchphrase.
/// Houdt ook bij wie vijanden zijn en wat de aartsvijand is. Een Guardian kan
/// meerdere collecties beheren die invloed hebben op gevechten.
/// </summary>
public abstract class Guardian
{
    public int Id { get; }
    public string Name { get; private set; }
    public string Location { get; private set; }
    public int PowerLevel { get; private set; }
    public string Catchphrase { get; private set; }
    public GuardianType Type { get; }

    private readonly List<Guardian> _enemies = new();
    public IReadOnlyList<Guardian> Enemies => _enemies;
    public Guardian? ArchEnemy { get; private set; }

    private readonly List<Collection> _collections = new();
    public IReadOnlyList<Collection> Collections => _collections;

    /// <summary>
    /// Maakt een nieuwe Guardian aan met basisgegevens. Als er geen catchphrase is opgegeven,
    /// wordt een standaardzin gebruikt. PowerLevel kan niet negatief zijn.
    /// </summary>
    /// <param name="id">Uniek identificatienummer.</param>
    /// <param name="name">Naam van de Guardian.</param>
    /// <param name="location">Locatie waar de Guardian actief is.</param>
    /// <param name="powerLevel">Basis kracht van de Guardian.</param>
    /// <param name="catchphrase">Slogan om tegenstanders af te schrikken (optioneel).</param>
    /// <param name="type">Het type Guardian.</param>
    protected Guardian(int id, string name, string location, int powerLevel, string? catchphrase, GuardianType type)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name must not be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("location must not be empty", nameof(location));
        if (powerLevel < 0) throw new ArgumentOutOfRangeException(nameof(powerLevel));

        Id = id;
        Name = name;
        Location = location;
        PowerLevel = powerLevel;
        Catchphrase = string.IsNullOrWhiteSpace(catchphrase) ? "Knowledge is power, and I wield it." : catchphrase!;
        Type = type;
    }

    /// <summary>
    /// Voeg een vijand toe van een ander type. Zelfde type is niet toegestaan en
    /// zichzelf toevoegen uiteraard ook niet. Dubbele invoer wordt genegeerd.
    /// </summary>
    public void AddEnemy(Guardian enemy)
    {
        if (enemy == null) throw new ArgumentNullException(nameof(enemy));
        if (ReferenceEquals(enemy, this)) throw new InvalidOperationException("A guardian cannot be its own enemy.");
        if (enemy.Type == this.Type) throw new InvalidOperationException("Guardians cannot have enemies of the same type.");
        if (_enemies.Contains(enemy)) return;
        _enemies.Add(enemy);
    }

    /// <summary>
    /// Stel de aartsvijand in. Dit moet iemand van een ander type zijn.
    /// Wordt tevens toegevoegd aan de gewone vijandenlijst als die daar nog niet in staat.
    /// </summary>
    public void SetArchEnemy(Guardian enemy)
    {
        if (enemy == null) throw new ArgumentNullException(nameof(enemy));
        if (enemy.Type == this.Type) throw new InvalidOperationException("Arch-enemy must be of a different type.");
        if (ReferenceEquals(enemy, this)) throw new InvalidOperationException("A guardian cannot be their own arch-enemy.");
        ArchEnemy = enemy;
        if (!_enemies.Contains(enemy)) _enemies.Add(enemy);
    }

    /// <summary>
    /// Koppel een collectie aan deze Guardian. In dit model moet de collectie al deze
    /// Guardian als eigenaar hebben; we registreren hem alleen in de eigen lijst.
    /// </summary>
    public void AddCollection(Collection collection)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (!ReferenceEquals(collection.Owner, this))
            throw new InvalidOperationException("Collection is owned by a different guardian.");
        if (_collections.Contains(collection)) return;
        _collections.Add(collection);
    }

    /// <summary>
    /// In dit model kunnen collecties niet eigenaarloos worden. Gebruik TransferCollectionTo
    /// om eigenaarschap over te dragen aan een andere Guardian.
    /// </summary>
    public void RemoveCollection(Collection collection)
    {
        throw new InvalidOperationException("Collections cannot be detached from owner; transfer instead.");
    }

    /// <summary>
    /// Draag een collectie over naar een andere Guardian. Verwijdert uit de eigen lijst,
    /// zet de nieuwe eigenaar en registreert de collectie bij de nieuwe eigenaar.
    /// </summary>
    public void TransferCollectionTo(Collection collection, Guardian newOwner)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (newOwner == null) throw new ArgumentNullException(nameof(newOwner));
        if (!ReferenceEquals(collection.Owner, this))
            throw new InvalidOperationException("Only the current owner can transfer the collection.");
        if (ReferenceEquals(this, newOwner)) return;

        if (_collections.Remove(collection))
        {
            collection.TransferTo(newOwner);
            newOwner.AddCollection(collection);
        }
    }

    public override string ToString() => $"{Type} #{Id} {Name} ({Location}) PL:{PowerLevel}";
}


