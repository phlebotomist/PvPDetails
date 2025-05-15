namespace PvPDetails;

public class PvPEventHandlers
{
    public static void OnPVPDeath()
    {
    }
    public static void OnPVEDeath()
    {
    }

    public static void OnPvPHit(ulong attackerPlatformId, string attackerName, int attackerLvl, ulong defenderId, string defenderName,
    int defenderLvl, int abilityHash, int dmgAmount)
    {
        UpdateDmg(attackerPlatformId, dmgAmount, attackerName);
        UpdateDmgTaken(defenderId, dmgAmount, defenderName);
        // PlayerHitStore.AddHit(attackerPlatformId, attackerName, attackerLvl, defenderId, defenderName, defenderLvl, abilityHash, dmgAmount);

    }
    private static void UpdateDmg(ulong playerId, int dmgAmount, string name)
    {
        PlayerStatStore.PlayerStats.TryGetValue(playerId, out PlayerStats playerStats);
        Helpers.P($"called {playerId}");
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