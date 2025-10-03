namespace Codex.Application;

/// <summary>
/// Eenvoudige User volgens UML. In deze console-app gebruiken we hem niet voor auth,
/// maar hij representeert de actor die acties kan ondernemen.
/// </summary>
public sealed class User
{
    public int Id { get; }
    public string Username { get; }
    public string Role { get; }

    public User(int id, string username, string role)
    {
        Id = id;
        Username = username;
        Role = role;
    }

    public void CreateBattle() { }
    public void SelectGuardian() { }
    public void DetermineWinner() { }
}


