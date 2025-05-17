namespace PvPDetails;

public static class DebugTesting
{
    public static void Test_SendBasicKillMessage()
    {
        // ── mock data ───────────────────────────────────────────────
        ulong killerId = 1001UL;
        string killerName = "Zig";
        int killerLvl = 55;

        ulong victimId = 2002UL;
        string victimName = "Morph";
        int victimLvl = 52;

        (ulong id, string name, int lvl)[] assisters =
        {
            (3003UL, "Rust",   50),
            (3004UL, "Julia", 49)
        };
        // ────────────────────────────────────────────────────────────

        ChatAnnouncements.SendBasicKillMessage(
            killerId,
            killerName,
            killerLvl,
            victimId,
            victimName,
            victimLvl,
            assisters
        );
    }
}
