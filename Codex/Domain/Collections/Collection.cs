namespace Codex.Domain.Collections;

using System;
using Codex.Domain;
using Codex.Domain.Guardians;

/// <summary>
/// Basisklasse voor alle kenniscollecties. Elke collectie heeft een id, naam, omschrijving
/// en een optionele eigenaar (Guardian). Subklassen voegen hun unieke eigenschappen toe.
/// </summary>
public abstract class Collection
{
    public int Id { get; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guardian Owner { get; private set; }
    public CollectionType Type { get; }

    /// <summary>
    /// Maak een collectie aan met een verplichte eigenaar. Naam en omschrijving mogen niet leeg zijn.
    /// </summary>
    protected Collection(int id, string name, string description, Guardian owner, CollectionType type)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name must not be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("description must not be empty", nameof(description));

        Id = id;
        Name = name;
        Description = description;
        Type = type;
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        // Zorg dat de eigenaar deze collectie ook kent
        owner.AddCollection(this);
    }

    /// <summary>
    /// Interne overdracht naar een andere eigenaar, gebruikt door Guardian.TransferCollectionTo.
    /// </summary>
    internal void TransferTo(Guardian newOwner)
    {
        Owner = newOwner ?? throw new ArgumentNullException(nameof(newOwner));
    }

    public override string ToString() => $"{Type} #{Id} {Name} - {Description}";
}


