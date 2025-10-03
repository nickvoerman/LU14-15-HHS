namespace Codex.Domain.Collections;

using Codex.Domain;
using Codex.Domain.Guardians;

/// <summary>
/// Een Spellbook bevat magische kennis en heeft een aantal pagina's.
/// Dit is het natuurlijke collectietype voor Mages.
/// </summary>
public sealed class Spellbook : Collection
{
    public int Pages { get; }

    /// <summary>
    /// Maak een Spellbook aan met een aantal pagina's (kan niet negatief zijn) en een eigenaar.
    /// </summary>
    public Spellbook(int id, string name, string description, Guardian owner, int pages)
        : base(id, name, description, owner, CollectionType.Spellbook)
    {
        if (pages < 0) throw new System.ArgumentOutOfRangeException(nameof(pages));
        Pages = pages;
    }
}


