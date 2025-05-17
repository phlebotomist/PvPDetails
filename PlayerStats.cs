using System;

namespace PvPDetails;

public record struct PlayerStats(
    ulong PlatformId,
    string Name,
    int Kills,
    int Deaths,
    int Assists,
    int CurrentKillStreak,
    int HighestKillStreak,
    int PVEDeaths,
    int Damage,
    // int SpellDamage,
    // int WeaponDamage,
    int DamageTaken)
{
    // public int GetTotalDamage()
    // {
    // return SpellDamage + WeaponDamage;
    // }

    public int GetTotalDeaths()
    {
        return Deaths + PVEDeaths;
    }

    public int GetStat(string category)
    {
        return category.ToLower() switch
        {
            "kills" => Kills,
            "deaths" => GetTotalDeaths(),
            "assists" => Assists,
            "killstreak" => CurrentKillStreak,
            "highestkillstreak" => HighestKillStreak,
            "damage" => Damage,
            _ => -1
        };
    }
}
