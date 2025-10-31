Codex Console Applicatie

Testplan (beknopt)

- Doel: aantonen dat kernlogica correct werkt; streefdekking: minimaal 70%.
- Strategie: unit-tests met xUnit. Focus op pure services en domeinregels.
- Tools: xUnit, Microsoft.NET.Test.Sdk, coverlet.collector voor coverage.

Geteste onderdelen

- Berekening gevechtsscore: natuurlijke collecties verhogen score.
- Winnaarbepaling individueel en team: hogere score wint (deterministisch getest).
- Fanoverdracht na verlies: fans verplaatsen van verliezer naar winnaar.

Projecten

- `Codex`: console-app met domein, services en menu.
- `Codex.Tests`: xUnit testsuite.

Uitvoeren

```bash
dotnet build
dotnet run --project Codex
dotnet test

# Tests met code coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
# Resultaten: ./TestResults/<guid>/coverage.cobertura.xml
```

Menu (Nederlands)

- Overzichten: Guardians, Collecties, Fans, Teams
- Gevechten: Individueel en Team
- Winnaar: automatisch met gewogen kans; deterministisch getest via override

Aanpasbare regels

- Catchphrase standaardtekst indien leeg
- Vijanden alleen van verschillend type; één aartsvijand
- Team: minimaal twee leden, zelfde type
- Fanoverdracht: standaard 1 fan per individueel gevecht (`FanService`)
