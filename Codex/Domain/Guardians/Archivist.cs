namespace Codex.Domain.Guardians;

using Codex.Domain;

/// <summary>
/// Een Archivist beheert fysieke documenten in Paper Archives en houdt bij hoeveel documenten er zijn.
/// Natuurlijk collectietype: PaperArchive.
/// </summary>
public sealed class Archivist : Guardian
{
    public int DocumentCount { get; private set; }

    /// <summary>
    /// Maak een Archivist aan met een aantal documenten. Aantal documenten kan niet negatief zijn.
    /// </summary>
    public Archivist(int id, string name, string location, int powerLevel, string? catchphrase, int documentCount)
        : base(id, name, location, powerLevel, catchphrase, GuardianType.Archivist)
    {
        if (documentCount < 0) throw new System.ArgumentOutOfRangeException(nameof(documentCount));
        DocumentCount = documentCount;
    }
}


