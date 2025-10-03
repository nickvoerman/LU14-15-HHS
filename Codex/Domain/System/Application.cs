namespace Codex.Application;

using Codex.Data;
using Codex.Domain.Battles;
using Codex.Domain.Guardians;
using Codex.Services;

/// <summary>
/// Simpele Application façade volgens UML. Roept services aan voor overzichten en berekeningen.
/// </summary>
public sealed class Application
{
    private readonly InMemoryStore _store;
    private readonly BattleService _battleService;
    private readonly FanService _fanService;

    public Application(InMemoryStore store, BattleService battleService, FanService fanService)
    {
        _store = store;
        _battleService = battleService;
        _fanService = fanService;
    }

    public void ShowOverview() { /* in Program.cs uitgewerkt in menu */ }
    public Guardian CalculateWinner(IndividualBattle battle) => _battleService.CalculateWinner(battle);
}


