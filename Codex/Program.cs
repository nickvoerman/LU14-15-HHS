using System;

using Codex.Data;
using Codex.Domain.Battles;
using Codex.Domain.Guardians;
using Codex.Services;

// InMemoryStore bevat onze voorbeelddata tijdens het draaien van de app
var store = new InMemoryStore();
store.Seed();

// Services voor fans en gevechten. GetFanCount is een delegate die we aan BattleService meegeven.
var fanService = new FanService();
int GetFanCount(Guardian g) => fanService.CountFansOf(g, store.Fans);
var battleService = new BattleService(GetFanCount);

while (true)
{
    // Hoofdmenu van de applicatie, eenvoudige consolebediening
    Console.WriteLine("==== Codex Console ====");
    Console.WriteLine("1) Overzicht Guardians");
    Console.WriteLine("2) Overzicht Collections");
    Console.WriteLine("3) Overzicht Fans");
    Console.WriteLine("4) Overzicht Teams");
    Console.WriteLine("5) Start Individueel Gevecht");
    Console.WriteLine("6) Start Team Gevecht");
    Console.WriteLine("0) Afsluiten");
    Console.Write("Keuze: ");
    var choice = Console.ReadLine();
    Console.WriteLine();

    if (choice == "0") break;

    switch (choice)
    {
        case "1":
            foreach (var g in store.Guardians)
            {
                Console.WriteLine(g);
                Console.WriteLine($"  Vijanden: {string.Join(", ", g.Enemies)}");
                Console.WriteLine($"  Aartsvijand: {g.ArchEnemy}");
                Console.WriteLine($"  Collecties: {string.Join(", ", g.Collections)}");
                Console.WriteLine($"  Fans: {GetFanCount(g)}");
            }
            Console.WriteLine();
            break;
        case "2":
            foreach (var c in store.Collections)
            {
                Console.WriteLine($"{c} | Eigenaar: {c.Owner}");
            }
            Console.WriteLine();
            break;
        case "3":
            foreach (var f in store.Fans)
            {
                Console.WriteLine($"{f.Id} {f.Name} -> {string.Join(", ", f.FavoriteGuardians)}");
            }
            Console.WriteLine();
            break;
        case "4":
            foreach (var t in store.Teams)
            {
                Console.WriteLine($"{t.Id} {t.Name} ({t.TeamType}) leden: {string.Join(", ", t.Members)}");
            }
            Console.WriteLine();
            break;
        case "5":
        {
            // Laat alleen paren zien van verschillende typen, want dat zijn geldige gevechten
            var differentTypePairs = new System.Collections.Generic.List<(Guardian, Guardian)>();
            for (int i = 0; i < store.Guardians.Count; i++)
            {
                for (int j = i + 1; j < store.Guardians.Count; j++)
                {
                    if (store.Guardians[i].Type != store.Guardians[j].Type)
                        differentTypePairs.Add((store.Guardians[i], store.Guardians[j]));
                }
            }
            for (int idx = 0; idx < differentTypePairs.Count; idx++)
            {
                Console.WriteLine($"{idx}) {differentTypePairs[idx].Item1} tegen {differentTypePairs[idx].Item2}");
            }
            Console.Write("Kies duel: ");
            if (int.TryParse(Console.ReadLine(), out var duelIdx) && duelIdx >= 0 && duelIdx < differentTypePairs.Count)
            {
                var (a, b) = differentTypePairs[duelIdx];
                var battle = battleService.CreateIndividualBattle(1, DateTime.Now, "Arena", a, b);
                Console.WriteLine($"Winnaar: {battle.Winner}");
                var loser = battle.Loser;
                fanService.TransferFans(loser, battle.Winner, store.Fans, 1); // verliezer verliest 1 fan aan de winnaar
            }
            Console.WriteLine();
            break;
        }
        case "6":
        {
            var teams = store.Teams;
            for (int i = 0; i < teams.Count; i++) Console.WriteLine($"{i}) {teams[i].Name} ({teams[i].TeamType})");
            Console.Write("Kies Team A index: ");
            if (!int.TryParse(Console.ReadLine(), out var aIdx) || aIdx < 0 || aIdx >= teams.Count) break;
            Console.Write("Kies Team B index: ");
            if (!int.TryParse(Console.ReadLine(), out var bIdx) || bIdx < 0 || bIdx >= teams.Count) break;
            var teamA = teams[aIdx];
            var teamB = teams[bIdx];
            if (teamA.TeamType == teamB.TeamType) { Console.WriteLine("Teams moeten een verschillend type hebben."); break; }
            var tBattle = battleService.CreateTeamBattle(2, DateTime.Now, "Grand Arena", teamA, teamB);
            Console.WriteLine($"Winnaar: {tBattle.Winner.Name}");
            Console.WriteLine();
            break;
        }
        default:
            Console.WriteLine("Ongeldige keuze.\n");
            break;
    }
}
