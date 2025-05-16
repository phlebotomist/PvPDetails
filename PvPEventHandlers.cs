using System;
using System.Collections.Generic;

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
        Dictionary<ulong, (string, int)> allAttackers)
    {
        UpdateKills(killerId);
        UpdateDeaths(victimId);
        List<(ulong, string, int)> assists = UpdateAssists(killerId, allAttackers);


        if (Settings.CombatBreakdownDetail == 1)
        {
            HookAnnouncements.SendSimpleKillReport(
                    PlayerStatStore.PlayerStats[killerId],
                    killerLvl,
                    PlayerStatStore.PlayerStats[victimId],
                    victimLvl,
                    assists.ToArray());
        }
        else if (Settings.CombatBreakdownDetail == 2)
        {
            HookAnnouncements.SendFightSummary(
                    PlayerStatStore.PlayerStats[killerId],
                    killerLvl,
                    PlayerStatStore.PlayerStats[victimId],
                    victimLvl,
                    assists.ToArray());
        }
        else if (Settings.CombatBreakdownDetail == 3)
        {
            HookAnnouncements.SendDetailedBreakdown(
                    PlayerStatStore.PlayerStats[killerId],
                    killerLvl,
                    PlayerStatStore.PlayerStats[victimId],
                    victimLvl,
                    assists.ToArray()
            );
        }

        ChatAnnouncements.SendBasicKillMessage(killerId, killerName, killerLvl, victimId, victimName, victimLvl, assists.ToArray());

    }
    public static void OnPvEDeath()
    {
        if (1 == 1)
            Helpers.P("TODO PVE DEATH");
    }

    public static void OnPvPHit(ulong attackerId, string attackerName, int attackerLvl, ulong defenderId, string defenderName,
    int defenderLvl, int abilityHash, int dmgAmount)
    {
        UpdateDmg(attackerId, dmgAmount, attackerName);
        UpdateDmgTaken(defenderId, dmgAmount, defenderName);
        PlayerHitStore.AddHit(attackerId, attackerName, attackerLvl, defenderId, defenderName, defenderLvl, abilityHash, dmgAmount);

    }

    private static void UpdateDeaths(ulong playerId)
    {
        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats playerStats);
        playerStats.PlatformId = playerId;
        playerStats.Deaths++;
        playerStats.CurrentKillStreak = 0;
        PlayerStatStore.PlayerStats[playerId] = playerStats;
    }
    private static void UpdateKills(ulong playerId)
    {
        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats ps);
        ps.PlatformId = playerId;
        ps.Kills++;
        ps.CurrentKillStreak++;
        ps.HighestKillStreak = Math.Max(ps.CurrentKillStreak, ps.HighestKillStreak);
        PlayerStatStore.PlayerStats[playerId] = ps;
    }
    private static void UpdateAssist(ulong playerId)
    {
        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats playerStats);
        playerStats.PlatformId = playerId;
        playerStats.Assists++;
        PlayerStatStore.PlayerStats[playerId] = playerStats;
    }
    private static List<(ulong, string, int)> UpdateAssists(ulong killerId, Dictionary<ulong, (string, int)> allAttackers)
    {
        var assists = new List<(ulong, string, int)>();
        foreach (ulong attackerId in allAttackers.Keys)
        {
            if (attackerId == killerId)
                continue;
            UpdateAssist(attackerId);
            assists.Add((attackerId, allAttackers[attackerId].Item1, allAttackers[attackerId].Item2));
        }
        return assists;
    }

    private static void UpdateDmg(ulong playerId, int dmgAmount, string name)
    {
        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats playerStats);
        playerStats.PlatformId = playerId;
        playerStats.Name = name;
        playerStats.Damage += dmgAmount;
        PlayerStatStore.PlayerStats[playerId] = playerStats;
    }
    private static void UpdateDmgTaken(ulong playerId, int dmgAmount, string name)
    {

        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats playerStats);
        playerStats.PlatformId = playerId;
        playerStats.Name = name;
        playerStats.DamageTaken += dmgAmount;
        PlayerStatStore.PlayerStats[playerId] = playerStats;
    }
}