namespace Codex.Domain.Collections;

using Codex.Domain;
using Codex.Domain.Guardians;

/// <summary>
/// Een Server Cluster vertegenwoordigt digitale rekencapaciteit (CPU cores en RAM in GB).
/// Dit is het natuurlijke collectietype voor Technomancers.
/// </summary>
public sealed class ServerCluster : Collection
{
    public int CpuCores { get; }
    public int RamGb { get; }

    /// <summary>
    /// Maak een Server Cluster aan met CPU-cores en RAM (beide niet negatief) en een eigenaar.
    /// </summary>
    public ServerCluster(int id, string name, string description, Guardian owner, int cpuCores, int ramGb)
        : base(id, name, description, owner, CollectionType.ServerCluster)
    {
        if (cpuCores < 0) throw new System.ArgumentOutOfRangeException(nameof(cpuCores));
        if (ramGb < 0) throw new System.ArgumentOutOfRangeException(nameof(ramGb));
        CpuCores = cpuCores;
        RamGb = ramGb;
    }
}


