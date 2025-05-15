namespace PvPDetails;

public class PvPEventHandlers
{
    public static void OnPvPDeath(
        ulong killerId,
        string killerName,
        int killerLvl,
        ulong victimId,
        string victimName,
        int victimLvl,
        ulong[] allAttackers)
    {
        if (1 == 1)
            Helpers.P("TODO PVP DEATH");
    }
    public static void OnPvEDeath()
    {
        if (1 == 1)
            Helpers.P("TODO PVE DEATH");
    }

    public static void OnPvPHit(ulong attackerPlatformId, string attackerName, int attackerLvl, ulong defenderId, string defenderName,
    int defenderLvl, int abilityHash, int dmgAmount)
    {
        UpdateDmg(attackerPlatformId, dmgAmount, attackerName);
        UpdateDmgTaken(defenderId, dmgAmount, defenderName);
        PlayerHitStore.AddHit(attackerPlatformId, attackerName, attackerLvl, defenderId, defenderName, defenderLvl, abilityHash, dmgAmount);

    }

    private static void UpdateDeaths(ulong playerId)
    {
        if (1 == 1)
            Helpers.P("TODO UpdateDeaths");
    }
    private static void UpdateKills(ulong playerId)
    {
        if (1 == 1)
            Helpers.P("TODO UpdateKills");
    }
    private static void UpdateAssists(ulong playerId)
    {
        if (1 == 1)
            Helpers.P("TODO UpdateAssists");
    }

    private static void UpdateDmg(ulong playerId, int dmgAmount, string name)
    {
        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats playerStats);
        playerStats.Name = name;
        playerStats.Damage += dmgAmount;
        PlayerStatStore.PlayerStats[playerId] = playerStats;
    }
    private static void UpdateDmgTaken(ulong playerId, int dmgAmount, string name)
    {

        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats playerStats);
        playerStats.Name = name;
        playerStats.DamageTaken += dmgAmount;
        PlayerStatStore.PlayerStats[playerId] = playerStats;
    }
}