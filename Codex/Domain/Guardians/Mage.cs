namespace Codex.Domain.Guardians;

using System.Collections.Generic;
using Codex.Domain;

/// <summary>
/// Een Mage beheerst arcane knowledge via Spellbooks. Heeft een lijst van spreuken en een mana-reserve.
/// Natuurlijke collectietype voor een Mage is het Spellbook.
/// </summary>
public sealed class Mage : Guardian
{
    private readonly List<string> _spells = new();
    public IReadOnlyList<string> Spells => _spells;
    public int ManaReserve { get; private set; }

    /// <summary>
    /// Maak een nieuwe Mage met een bepaalde mana-reserve. Mana mag niet negatief zijn.
    /// </summary>
    public Mage(int id, string name, string location, int powerLevel, string? catchphrase, int manaReserve)
        : base(id, name, location, powerLevel, catchphrase, GuardianType.Mage)
    {
        if (manaReserve < 0) throw new System.ArgumentOutOfRangeException(nameof(manaReserve));
        ManaReserve = manaReserve;
    }

    /// <summary>
    /// Laat de Mage een nieuwe spreuk leren. Lege invoer of dubbele spreuken worden genegeerd.
    /// </summary>
    public void LearnSpell(string spell)
    {
        if (string.IsNullOrWhiteSpace(spell)) return;
        if (!_spells.Contains(spell)) _spells.Add(spell);
    }
}


