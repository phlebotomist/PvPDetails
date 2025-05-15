using System;

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
        UpdateKills(killerId);
        UpdateDeaths(victimId);
        UpdateAssists(killerId, allAttackers);
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
        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats playerStats);
        playerStats.Deaths++;
        playerStats.CurrentKillStreak = 0;
        PlayerStatStore.PlayerStats[playerId] = playerStats;
    }
    private static void UpdateKills(ulong playerId)
    {
        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats ps);
        ps.Kills++;
        ps.CurrentKillStreak++;
        ps.HighestKillStreak = Math.Max(ps.CurrentKillStreak, ps.HighestKillStreak);
        PlayerStatStore.PlayerStats[playerId] = ps;
    }
    private static void UpdateAssist(ulong playerId)
    {
        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats playerStats);
        playerStats.Assists++;
        PlayerStatStore.PlayerStats[playerId] = playerStats;
    }
    private static void UpdateAssists(ulong killerId, ulong[] allAttackers)
    {
        foreach (ulong attacker in allAttackers)
        {
            if (attacker == killerId)
                continue;
            UpdateAssist(attacker);
        }
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