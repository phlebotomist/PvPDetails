using System.Collections.Generic;

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

    public static void Test_SendSimpleKillReport()
    {
        // ── mock data ───────────────────────────────────────────────
        var killer = new PlayerStats(
            1101UL,       // PlatformId
            "Zig",       // Name
            Kills: 5,
            Deaths: 1,
            Assists: 2,
            CurrentKillStreak: 3,
            HighestKillStreak: 4,
            PVEDeaths: 0,
            Damage: 1245,
            DamageTaken: 300
        );
        int killerLvl = 60;

        var victim = new PlayerStats(
            2202UL,
            "Morph",
            Kills: 2,
            Deaths: 3,
            Assists: 0,
            CurrentKillStreak: 1,
            HighestKillStreak: 2,
            PVEDeaths: 1,
            Damage: 900,
            DamageTaken: 150
        );
        int victimLvl = 58;

        (ulong, string, int)[] assisters =
        {
            (3003UL, "Rust",  50),
            (3004UL, "Julia", 49)
        };
        // ────────────────────────────────────────────────────────────

        HookAnnouncements.SendSimpleKillReport(
            killer,
            killerLvl,
            victim,
            victimLvl,
            assisters
        );
    }

    public static void Test_SendDetailedBreakdown()
    {
        // ── mock data ───────────────────────────────────────────────
        var victim = new PlayerStats(
            2402UL,
            "Morph",
            Kills: 4,
            Deaths: 5,
            Assists: 1,
            CurrentKillStreak: 2,
            HighestKillStreak: 3,
            PVEDeaths: 0,
            Damage: 800,
            DamageTaken: 500
        );
        int victimLvl = 63;

        var killer = new PlayerStats(
            1301UL,
            "Zig",
            Kills: 10,
            Deaths: 2,
            Assists: 3,
            CurrentKillStreak: 5,
            HighestKillStreak: 7,
            PVEDeaths: 1,
            Damage: 1500,
            DamageTaken: 400
        );
        int killerLvl = 65;

        (ulong, string, int)[] assisters =
        {
            (3003UL, "Rust",  50),
            (3004UL, "Julia", 49)
        };

        // ── mock hit interactions ────────────────────────────────────
        IReadOnlyList<HitInteraction> mockHits = new List<HitInteraction>
        {
            new HitInteraction
            {
                AttackerSteamId = 1301UL,
                VictimSteamId = 2402UL,
                AttackerName = "Zig",
                AttackerLevel = 65,
                VictimName = "Morph",
                VictimLevel = 63,
                Timestamp = 1000000L,
                DmgSourceGUID = 101,
                DmgAmount = 250
            },
            new HitInteraction
            {
                AttackerSteamId = 2402UL,
                VictimSteamId = 1301UL,
                AttackerName = "Morph",
                AttackerLevel = 63,
                VictimName = "Zig",
                VictimLevel = 65,
                Timestamp = 1000500L,
                DmgSourceGUID = 102,
                DmgAmount = 150
            },
            new HitInteraction
            {
                AttackerSteamId = 1301UL,
                VictimSteamId = 2402UL,
                AttackerName = "Zig",
                AttackerLevel = 65,
                VictimName = "Morph",
                VictimLevel = 63,
                Timestamp = 1001900L,
                DmgSourceGUID = 101,
                DmgAmount = 250
            }
        };
        // ────────────────────────────────────────────────────────────

        HookAnnouncements.SendDetailedBreakdown(
            victim,
            victimLvl,
            killer,
            killerLvl,
            assisters,
            mockHits // Mocked hits
        );
    }

    public static void Test_SendFightSummary()
    {
        // ── mock data ───────────────────────────────────────────────
        var killer = new PlayerStats(
            1501UL,
            "Zig",
            Kills: 8,
            Deaths: 3,
            Assists: 2,
            CurrentKillStreak: 4,
            HighestKillStreak: 6,
            PVEDeaths: 0,
            Damage: 1300,
            DamageTaken: 350
        );
        int killerLvl = 70;

        var victim = new PlayerStats(
            2502UL,
            "Morph",
            Kills: 2,
            Deaths: 6,
            Assists: 1,
            CurrentKillStreak: 1,
            HighestKillStreak: 2,
            PVEDeaths: 1,
            Damage: 900,
            DamageTaken: 550
        );
        int victimLvl = 68;

        (ulong, string, int)[] assisters =
        {
            (3503UL, "Rust",  66),
            (3504UL, "Julia", 65)
        };
        // ── mock hit interactions ────────────────────────────────────
        IReadOnlyList<HitInteraction> mockHits = new List<HitInteraction>
        {
            new HitInteraction
            {
                AttackerSteamId = 1301UL,
                VictimSteamId = 2402UL,
                AttackerName = "Zig",
                AttackerLevel = 65,
                VictimName = "Morph",
                VictimLevel = 63,
                Timestamp = 1000000L,
                DmgSourceGUID = 101,
                DmgAmount = 250
            },
            new HitInteraction
            {
                AttackerSteamId = 2402UL,
                VictimSteamId = 1301UL,
                AttackerName = "Morph",
                AttackerLevel = 63,
                VictimName = "Zig",
                VictimLevel = 65,
                Timestamp = 1000500L,
                DmgSourceGUID = 102,
                DmgAmount = 150
            },
            new HitInteraction
            {
                AttackerSteamId = 1301UL,
                VictimSteamId = 2402UL,
                AttackerName = "Zig",
                AttackerLevel = 65,
                VictimName = "Morph",
                VictimLevel = 63,
                Timestamp = 1001900L,
                DmgSourceGUID = 101,
                DmgAmount = 250
            }
        };
        // ────────────────────────────────────────────────────────────

        HookAnnouncements.SendFightSummary(
            killer,
            killerLvl,
            victim,
            victimLvl,
            assisters,
            mockHits
        );
    }
}
