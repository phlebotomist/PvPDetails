using System;
using System.Linq;
using VampireCommandFramework;
namespace PvPDetails;

public class Commands
{
    private enum PvPCategories
    {
        Kills,
        // Deaths,
        Assists,
        // KillStreak,
        Damage,
        // DamageTaken
    }
    [Command("pvpdetails", shortHand: "pd_", description: "test plugin is loaded loaded", adminOnly: true)]
    public void testpvpdetails(ChatCommandContext ctx)
    {
        ctx.Reply("pvpdetails loaded happily!");
    }

    [Command("pvptop", shortHand: "ptop", description: "prints the players stats for x category")]
    public void PvPStats(ChatCommandContext ctx, string category)
    {
        if (!Enum.TryParse(category, ignoreCase: true, out PvPCategories choice))
        {
            ctx.Reply("Valid categories are: Kills, Assists, Damage");
            return;
        }

        if (PlayerStatStore.PlayerStats.Count == 0)
        {
            ctx.Reply("oh no! no one has played yet!");
            return;
        }

        ctx.Reply($"here's the top players for {category}!");

        var sorted = PlayerStatStore.PlayerStats.Values
            .OrderByDescending(x => x.GetStat(category))
            .ToList();
        for (int i = 0; i < Math.Min(5, sorted.Count); i++)
        {
            var playerStats = sorted[i];
            ctx.Reply($"{i + 1}. {playerStats.Name} - {playerStats.GetStat(category)}");
        }
    }

    [Command("pvpme", description: "prints the players stats")]
    public void PvPMe(ChatCommandContext ctx)
    {
        ctx.Reply($"here's your stats {ctx.Name}!");
        if (PlayerStatStore.PlayerStats.TryGetValue(ctx.User.PlatformId, out PlayerStats playerStats))
        {
            ctx.Reply($"Name: {playerStats.Name}");
            ctx.Reply($"Kills: {playerStats.Kills}");
            ctx.Reply($"Deaths: {playerStats.GetTotalDeaths()}");
            ctx.Reply($"Assists: {playerStats.Assists}");
            ctx.Reply($"Current Kill Streak: {playerStats.CurrentKillStreak}");
            ctx.Reply($"Highest Kill Streak: {playerStats.HighestKillStreak}");
            ctx.Reply($"Damage: {playerStats.Damage}");
            ctx.Reply($"Damage Taken: {playerStats.DamageTaken}");
        }
        else
        {
            ctx.Reply("oh no! you don't exists silly!");
        }

    }
}