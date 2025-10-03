namespace Codex.Domain.Collections;

using Codex.Domain;
using Codex.Domain.Guardians;

/// <summary>
/// Een Paper Archive vertegenwoordigt fysieke documenten, gemeten in aantal boekenplanken.
/// Dit is het natuurlijke collectietype voor Archivists.
/// </summary>
public sealed class PaperArchive : Collection
{
    public int Shelves { get; }

    /// <summary>
    /// Maak een Paper Archive aan met een aantal boekenplanken (kan niet negatief zijn) en een eigenaar.
    /// </summary>
    public PaperArchive(int id, string name, string description, Guardian owner, int shelves)
        : base(id, name, description, owner, CollectionType.PaperArchive)
    {
        if (shelves < 0) throw new System.ArgumentOutOfRangeException(nameof(shelves));
        Shelves = shelves;
    }
}


