namespace Codex.Domain.Teams;

using System;
using System.Collections.Generic;
using System.Linq;
using Codex.Domain.Guardians;

/// <summary>
/// Een Team bestaat uit minimaal twee leden van hetzelfde Guardian-type.
/// Teams hebben een id, naam en een ledenlijst. TeamType is afgeleid van het type van het eerste lid.
/// </summary>
public sealed class Team
{
    public int Id { get; }
    public string Name { get; }
    private readonly List<Guardian> _members = new();
    public IReadOnlyList<Guardian> Members => _members;

    /// <summary>
    /// Maak een Team aan. Leden kunnen optioneel meteen toegevoegd worden; de type-check gebeurt bij toevoegen.
    /// </summary>
    public Team(int id, string name, IEnumerable<Guardian>? initialMembers = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name must not be empty", nameof(name));
        Id = id;
        Name = name;
        if (initialMembers != null)
        {
            foreach (var m in initialMembers) AddMember(m);
            if (_members.Count < 2) throw new InvalidOperationException("Team must have at least two members.");
        }
    }

    /// <summary>
    /// Type van het team (gebaseerd op het eerste lid), of null wanneer er nog geen leden zijn.
    /// </summary>
    public GuardianType? TeamType => _members.Count == 0 ? null : _members[0].Type;

    /// <summary>
    /// Voeg een lid toe. Alle leden moeten van hetzelfde type zijn. Dubbele leden worden genegeerd.
    /// </summary>
    public void AddMember(Guardian guardian)
    {
        if (guardian == null) throw new ArgumentNullException(nameof(guardian));
        if (_members.Count > 0 && guardian.Type != _members[0].Type)
        {
            throw new InvalidOperationException("All team members must be of the same Guardian type.");
        }
        if (_members.Contains(guardian)) return;
        _members.Add(guardian);
        if (_members.Count < 2)
        {
            // Allow building up; the minimum size rule should be checked when using the team in battles or listings
        }
    }

    /// <summary>
    /// Geldigheidscheck: team moet minstens 2 leden hebben.
    /// </summary>
    public bool IsValidTeam() => _members.Count >= 2;
}


