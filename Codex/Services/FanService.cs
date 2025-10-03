namespace Codex.Services;

using System;
using System.Collections.Generic;
using Codex.Domain.Fans;
using Codex.Domain.Guardians;

/// <summary>
/// Service voor fans: tellen hoeveel fans een Guardian heeft en fans overdragen na een gevecht.
/// </summary>
public sealed class FanService
{
    /// <summary>
    /// Tel het aantal fans dat een bepaalde Guardian volgt.
    /// </summary>
    public int CountFansOf(Guardian guardian, IEnumerable<Fan> fans)
    {
        if (guardian == null) throw new ArgumentNullException(nameof(guardian));
        if (fans == null) throw new ArgumentNullException(nameof(fans));
        int count = 0;
        foreach (var fan in fans)
        {
            foreach (var fav in fan.FavoriteGuardians)
            {
                if (ReferenceEquals(fav, guardian)) { count++; break; }
            }
        }
        return count;
    }

    /// <summary>
    /// Verplaats een aantal fans (count) van verliezer naar winnaar. Loopt simpelweg de lijst door
    /// en pakt de eerste fans die de verliezer volgen. Retourneert hoeveel er echt verplaatst zijn.
    /// </summary>
    public int TransferFans(Guardian from, Guardian to, IList<Fan> fans, int count)
    {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (to == null) throw new ArgumentNullException(nameof(to));
        if (fans == null) throw new ArgumentNullException(nameof(fans));
        if (count <= 0) return 0;
        int moved = 0;
        foreach (var fan in fans)
        {
            bool likedFrom = false;
            foreach (var g in fan.FavoriteGuardians)
            {
                if (ReferenceEquals(g, from)) { likedFrom = true; break; }
            }
            if (likedFrom)
            {
                fan.Unfollow(from);
                fan.Follow(to);
                moved++;
                if (moved >= count) break;
            }
        }
        return moved;
    }
}


