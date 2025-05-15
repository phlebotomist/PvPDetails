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
}
