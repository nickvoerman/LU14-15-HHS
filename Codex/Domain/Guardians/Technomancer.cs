namespace Codex.Domain.Guardians;

using System.Collections.Generic;
using Codex.Domain;

/// <summary>
/// Een Technomancer controleert digitale kennis via Server Clusters en onderhoudt een lijst met algoritmen.
/// Natuurlijk collectietype: ServerCluster.
/// </summary>
public sealed class Technomancer : Guardian
{
    private readonly List<string> _algorithms = new();
    public IReadOnlyList<string> Algorithms => _algorithms;

    /// <summary>
    /// Maak een Technomancer aan. Catchphrase is optioneel en valt terug op een standaardzin.
    /// </summary>
    public Technomancer(int id, string name, string location, int powerLevel, string? catchphrase)
        : base(id, name, location, powerLevel, catchphrase, GuardianType.Technomancer)
    {
    }

    /// <summary>
    /// Voeg een algoritme toe. Lege invoer of dubbelen worden genegeerd.
    /// </summary>
    public void AddAlgorithm(string algorithm)
    {
        if (string.IsNullOrWhiteSpace(algorithm)) return;
        if (!_algorithms.Contains(algorithm)) _algorithms.Add(algorithm);
    }
}


