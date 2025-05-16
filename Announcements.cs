using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VampireWebhook;

namespace PvPDetails;

public static class HookAnnouncements
{
    private static string GetAssistNameAndLvl((ulong, string, int) assist)
    {
        return $"**{assist.Item2}** ({assist.Item3})";
    }
    public static string GetAssistString((ulong, string, int)[] assisters)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"**Assisters:** ");
        for (int i = 0; i < assisters.Length; i++)
        {
            var assist = assisters[i];
            sb.Append($"• {GetAssistNameAndLvl(assist)}");
            sb.Append(" •");
        }
        return sb.ToString();
    }
    private static string GetKillString(PlayerStats killer, int killerLvl, PlayerStats victim, int victimLvl)
    {
        return $"🗡️ **{killer.Name}** ({killerLvl}) killed **{victim.Name}** ({victimLvl}) ☠️";
    }

    /// <summary>
    /// Builds a simple Markdown kill report and sends it.
    /// </summary>
    public static void SendSimpleKillReport(PlayerStats killer, int killerLvl, PlayerStats victim, int victimLvl, (ulong, string, int)[] assisters)
    {
        if (!DiscordWebhook.HookEnabled())
            return;

        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine(GetKillString(killer, killerLvl, victim, victimLvl));
        if (assisters != null && assisters.Length > 0)
        {
            sb.AppendLine(GetAssistString(assisters));
        }

        _ = DiscordWebhook.SendDiscordMessageAsync(sb.ToString());
    }


    /// <summary>
    /// sends hit by hit breakdown to the webhook.
    /// </summary>
    public static void SendDetailedBreakdown(
        PlayerStats victim,
        int victimLvl,
        PlayerStats killer,
        int killerLvl,
        (ulong, string, int)[] assisters,
        double pvpWindowSeconds = 30.0)
    {
        if (!DiscordWebhook.HookEnabled())
            return;

        var headerSb = new StringBuilder();
        headerSb.Append(GetKillString(killer, killerLvl, victim, victimLvl));
        if (assisters != null && assisters.Length > 0)
        {
            headerSb.AppendLine(GetAssistString(assisters));
        }

        // Fetch recent hits
        var hits = PlayerHitStore.GetRecentInteractions(victim.PlatformId, pvpWindowSeconds);

        // Build breakdown lines
        var sb = new StringBuilder();
        sb.AppendLine(headerSb.ToString());
        sb.AppendLine(); // blank line
        sb.AppendLine($"📊 __hit by hit breakdown breakdown for__ **{victim.Name}** (last {pvpWindowSeconds:F0}s)");

        foreach (var hit in hits)
        {
            var ability = HitNameResolver.Resolve(hit.DmgSourceGUID);
            sb.AppendLine(
                $"• **{hit.AttackerName}** ({hit.AttackerLevel}) " +
                $"hit **{hit.VictimName}** ({hit.VictimLevel}) " +
                $"with **{ability}**" +
                $" for **{hit.DmgAmount}** damage"
            );
        }

        _ = DiscordWebhook.SendDiscordMessageAsync(sb.ToString());
    }

    public static void SendFightSummary(
    PlayerStats killer,
    int killerLvl,
    PlayerStats victim,
    int victimLvl,
    (ulong, string, int)[] assisters,
    double pvpWindowSeconds = 30.0)
    {
        if (!DiscordWebhook.HookEnabled())
            return;

        var hits = PlayerHitStore.GetRecentInteractions(victim.PlatformId, pvpWindowSeconds);
        var incoming = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var outgoing = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        foreach (var hit in hits)
        {
            var ability = HitNameResolver.Resolve(hit.DmgSourceGUID);

            //TODO: We prob need to deal with sun and silve here as well
            if (string.Equals(hit.VictimName, victim.Name, StringComparison.OrdinalIgnoreCase))
            {
                incoming[ability] = incoming.TryGetValue(ability, out var v) ? v + hit.DmgAmount : hit.DmgAmount;
            }
            else if (string.Equals(hit.AttackerName, victim.Name, StringComparison.OrdinalIgnoreCase))
            {
                outgoing[ability] = outgoing.TryGetValue(ability, out var v) ? v + hit.DmgAmount : hit.DmgAmount;
            }
        }

        var sb = new StringBuilder();

        sb.AppendLine(GetKillString(killer, killerLvl, victim, victimLvl));
        sb.AppendLine();
        if (incoming.Count > 0)
        {
            sb.AppendLine($"📊 __Incoming damage for__ **{victim.Name}** (last {pvpWindowSeconds:F0}s)");
            foreach (var kvp in incoming.OrderByDescending(k => k.Value))
                sb.AppendLine($"• {kvp.Key}: **{kvp.Value:F0}**");
        }

        if (outgoing.Count > 0)
        {
            sb.AppendLine(); // blank line between the two sections
            sb.AppendLine($"📊 __Outgoing damage for__ **{victim.Name}** (last {pvpWindowSeconds:F0}s)");
            foreach (var kvp in outgoing.OrderByDescending(k => k.Value))
                sb.AppendLine($"• {kvp.Key}: **{kvp.Value:F0}**");
        }

        _ = DiscordWebhook.SendDiscordMessageAsync(sb.ToString());
    }
}

public static class ChatAnnouncements
{
    public static void SendBasicKillMessage(
        ulong killerId,
        string killerName,
        int killerLvl,
        ulong victimId,
        string victimName,
        int victimLvl,
        (ulong,
        string, int)[] assisters)
    {
        string assistString = "";
        if (assisters != null && assisters.Length > 0)
        {
            assistString = HookAnnouncements.GetAssistString(assisters);
        }
        Helpers.P($"{killerName} ({killerLvl}) killed {victimName} ({victimLvl})! {assistString}");
    }
}