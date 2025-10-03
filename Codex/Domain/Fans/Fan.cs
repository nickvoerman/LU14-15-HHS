namespace Codex.Domain.Fans;

using System;
using System.Collections.Generic;
using Codex.Domain.Guardians;

/// <summary>
/// Een Fan is een gewone persoon die één of meerdere Guardians kan volgen.
/// Bevat een naam en uniek id, en een verzameling favoriete Guardians.
/// </summary>
public sealed class Fan
{
    public int Id { get; }
    public string Name { get; }

    private readonly HashSet<Guardian> _favoriteGuardians = new();
    public IReadOnlyCollection<Guardian> FavoriteGuardians => _favoriteGuardians;

    /// <summary>
    /// Maak een Fan aan. Naam mag niet leeg zijn.
    /// </summary>
    public Fan(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name must not be empty", nameof(name));
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Laat de fan een Guardian volgen. Dubbele volgingen worden genegeerd door de HashSet.
    /// </summary>
    public void Follow(Guardian guardian)
    {
        if (guardian == null) throw new ArgumentNullException(nameof(guardian));
        _favoriteGuardians.Add(guardian);
    }

    /// <summary>
    /// Laat de fan stoppen met volgen van een Guardian.
    /// </summary>
    public void Unfollow(Guardian guardian)
    {
        if (guardian == null) throw new ArgumentNullException(nameof(guardian));
        _favoriteGuardians.Remove(guardian);
    }
}


