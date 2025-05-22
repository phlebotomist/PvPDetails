using System;
using System.Collections.Generic;
using System.Diagnostics;
using VampireDB;

namespace PvPDetails;

public class PlayerStatStore
{
    private const string DATA_VERSION = "v1.0";
    private const string DB_ID = MyPluginInfo.PLUGIN_GUID + DATA_VERSION;
    public static Dictionary<ulong, PlayerStats> PlayerStats = new();

    private static double totalSaveTime = 0;
    private static int count = 0;

    private const ulong DEBUG_ID = 0xDEADBEEF;
    private const string DEBUG_NAMES_KEY = $"{DB_ID}:badnames";

    public static void SaveData()
    {
        try
        {
            var sw = Stopwatch.StartNew();

            Storage.Instance.RunInTransaction(() =>
            {
                Dictionary<ulong, PlayerStats> snapshot;

                snapshot = new Dictionary<ulong, PlayerStats>(PlayerStats);
                foreach (var (id, stats) in snapshot)
                    Storage.Instance.Set(id, DB_ID, stats);

                // Save debug info
                var debugSnapshot = new HashSet<int>(HitNameResolver.FailedIds);
                Storage.Instance.Set(DEBUG_ID, DEBUG_NAMES_KEY, debugSnapshot);
            });


            sw.Stop();
            Plugin.L.LogInfo($"Saved {PlayerStats.Count} players && [DNames:{HitNameResolver.FailedIds.Count}] in {sw.ElapsedMilliseconds} ms");
            totalSaveTime += sw.ElapsedMilliseconds;
            count++;
            if (count % 10 == 0)
            {
                Plugin.L.LogInfo($"Average save time over last {count} saves: {totalSaveTime / count} ms");
                totalSaveTime = 0;// reset the total save time so we don't overflow anything
                count = 0;
            }
        }
        catch (Exception e)
        {
            Plugin.L.LogError($"Failed to save player stats: {e}");
            return;
        }
    }

    public static void LoadData()
    {
        try
        {
            var sw = Stopwatch.StartNew();

            PlayerStats = Storage.Instance.GetAll<PlayerStats>(DB_ID);

            // this is ugly. VampireDB is stupid
            Dictionary<ulong, HashSet<int>> ul_failedIds = Storage.Instance.GetAll<HashSet<int>>(DEBUG_NAMES_KEY);
            if (ul_failedIds.TryGetValue(DEBUG_ID, out var failedIds))
                HitNameResolver.FailedIds = failedIds;

            sw.Stop();
            Plugin.L.LogInfo($"Loaded {PlayerStats.Count} players && [DNames: {failedIds.Count}] in {sw.ElapsedMilliseconds} ms");
        }
        catch (Exception e)
        {
            Plugin.L.LogError($"Failed to load player stats: {e}");
        }
    }

}