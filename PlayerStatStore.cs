using System.Collections.Generic;

namespace PvPDetails;

public class PlayerStatStore
{
    public static Dictionary<ulong, PlayerStats> PlayerStats = new();

    public static void SaveData()
    {
        Helpers.P("TODO: PlayerStatStore.SaveData");
        // Save data todo
    }
}