using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PvPDetails;

public struct HitInteraction
{
    public ulong AttackerSteamId;
    public ulong VictimSteamId;
    public string AttackerName;
    public int AttackerLevel;
    public string VictimName;
    public int VictimLevel;
    public long Timestamp;
    public int DmgSourceGUID;
    public int DmgAmount;
}

public class PlayerHitData
{
    public List<HitInteraction> Attacks { get; } = new List<HitInteraction>();
    public List<HitInteraction> Defenses { get; } = new List<HitInteraction>();
}

public static class PlayerHitStore
{
    private const double PVP_WINDOW = 30.0;

    private static readonly Dictionary<ulong, PlayerHitData> interactionsByPlayer =
        new Dictionary<ulong, PlayerHitData>();

    public static IReadOnlyDictionary<ulong, PlayerHitData> InteractionsByPlayer => interactionsByPlayer;

    public static void AddHit(
        ulong attackerSteamId,
        string attackerName,
        int attackerLevel,
        ulong victimSteamId,
        string victimName,
        int victimLevel,
        int dmgSourceGUID,
        int damageAmount)
    {
        var hit = new HitInteraction
        {
            AttackerSteamId = attackerSteamId,
            AttackerName = attackerName,
            AttackerLevel = attackerLevel,
            VictimSteamId = victimSteamId,
            VictimName = victimName,
            VictimLevel = victimLevel,
            Timestamp = Stopwatch.GetTimestamp(),
            DmgSourceGUID = dmgSourceGUID,
            DmgAmount = damageAmount
        };

        AddAttack(attackerSteamId, hit);
        AddDefense(victimSteamId, hit);

        // cleanup if too many defenses
        if (interactionsByPlayer.TryGetValue(victimSteamId, out var victimHitData)
            && victimHitData.Defenses.Count >= 500)
        {
            CleanupOldHitInteractionsByPlayer(victimSteamId);
        }
    }

    private static void AddAttack(ulong playerSteamId, HitInteraction hit)
    {
        if (!interactionsByPlayer.TryGetValue(playerSteamId, out var hitData))
        {
            hitData = new PlayerHitData();
            interactionsByPlayer[playerSteamId] = hitData;
        }
        hitData.Attacks.Add(hit);
    }

    private static void AddDefense(ulong playerSteamId, HitInteraction hit)
    {
        if (!interactionsByPlayer.TryGetValue(playerSteamId, out var hitData))
        {
            hitData = new PlayerHitData();
            interactionsByPlayer[playerSteamId] = hitData;
        }
        hitData.Defenses.Add(hit);
    }

    public static IReadOnlyList<HitInteraction> GetAttacks(ulong playerSteamId)
    {
        if (interactionsByPlayer.TryGetValue(playerSteamId, out var hitData))
            return hitData.Attacks;
        return [];
    }

    public static IReadOnlyList<HitInteraction> GetDefenses(ulong playerSteamId)
    {
        if (interactionsByPlayer.TryGetValue(playerSteamId, out var hitData))
            return hitData.Defenses;
        return [];
    }

    /// <summary>
    /// Returns a dictionary mapping each attacker’s Steam ID to (name, highest level)
    /// for all hits on the target player within the last <paramref name="pvpWindowSeconds"/> seconds, excluding self-hits.
    /// </summary>
    public static Dictionary<ulong, (string Name, int Level)> GetRecentAttackersWithLvl(
        ulong playerSteamId,
        double pvpWindowSeconds = PVP_WINDOW)
    {
        var result = new Dictionary<ulong, (string, int)>();

        if (!interactionsByPlayer.TryGetValue(playerSteamId, out var hitData))
            return result;

        long currentTicks = Stopwatch.GetTimestamp();
        long windowTicks = (long)(pvpWindowSeconds * Stopwatch.Frequency);

        foreach (HitInteraction hit in hitData.Defenses)
        {
            if (currentTicks - hit.Timestamp > windowTicks) continue;

            if (playerSteamId == hit.AttackerSteamId) continue;

            if (result.TryGetValue(hit.AttackerSteamId, out (string, int) name_lvl))
            {
                if (hit.AttackerLevel > name_lvl.Item2)
                    result[hit.AttackerSteamId] = (hit.AttackerName, hit.AttackerLevel);
            }
            else
            {
                result[hit.AttackerSteamId] = (hit.AttackerName, hit.AttackerLevel);
            }
        }
        return result;
    }

    /// <summary>
    /// Returns a the highest level used by victim attacking the killer
    /// for all hits within the last <paramref name="pvpWindowSeconds"/> seconds.
    /// </summary>
    public static int GetHighestLvlUsedOnKiller(
        ulong victimSteamId,
        ulong killerSteamId,
        double pvpWindowSeconds = PVP_WINDOW)
    {
        int peakLevel = -1;

        if (!interactionsByPlayer.TryGetValue(victimSteamId, out var hitData))
            return peakLevel;

        long currentTicks = Stopwatch.GetTimestamp();
        long windowTicks = (long)(pvpWindowSeconds * Stopwatch.Frequency);

        foreach (HitInteraction hit in hitData.Attacks)
        {
            if (currentTicks - hit.Timestamp > windowTicks) continue;

            if (killerSteamId == hit.VictimSteamId)
            {
                peakLevel = Math.Max(peakLevel, hit.AttackerLevel);
            }
        }
        return peakLevel;
    }

    /// <summary>
    /// Returns all hit interactions (both attacks and defenses) for the given player
    /// that occurred within the 'pvpWindowSeconds' seconds,
    /// sorted by timestamp (oldest first).
    /// </summary>
    public static IReadOnlyList<HitInteraction> GetRecentInteractions(
        ulong playerSteamId,
        double pvpWindowSeconds = PVP_WINDOW)
    {
        if (!interactionsByPlayer.TryGetValue(playerSteamId, out var hitData))
            return [];

        long nowTicks = Stopwatch.GetTimestamp();
        long windowTicks = (long)(pvpWindowSeconds * Stopwatch.Frequency);
        long earliest = nowTicks - windowTicks;

        return hitData.Attacks
            .Concat(hitData.Defenses)
            .Where(hit => hit.Timestamp >= earliest)
            .OrderBy(hit => hit.Timestamp)
            .ToList();
    }

    public static void CleanupOldHitInteractionsByPlayer(
        ulong playerSteamId,
        double pvpWindowSeconds = PVP_WINDOW)
    {
        if (!interactionsByPlayer.TryGetValue(playerSteamId, out var hitData))
            return;

        Plugin.L.LogMessage($"CLEANING up hit interactions for SteamID: {playerSteamId}");

        long currentTicks = Stopwatch.GetTimestamp();
        long windowTicks = (long)(pvpWindowSeconds * Stopwatch.Frequency);

        int before = hitData.Defenses.Count;
        hitData.Defenses.RemoveAll(hit => (currentTicks - hit.Timestamp) > windowTicks);
        int after = hitData.Defenses.Count;
        Plugin.L.LogMessage($"CLEANED up {before - after} old hit interactions for SteamID: {playerSteamId}");
    }

    public static void ResetPlayerHitInteractions(ulong playerSteamId)
    {
        if (interactionsByPlayer.TryGetValue(playerSteamId, out var hitData))
        {
            hitData.Attacks.Clear();
            hitData.Defenses.Clear();
        }
    }

    public static void Clear() => interactionsByPlayer.Clear();
}
