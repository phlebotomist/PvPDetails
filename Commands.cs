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
            ctx.Reply($"{i + 1}. {playerStats.Name}: {playerStats.GetStat(category)}");
        }
    }

    [Command("pvpcombatlevel", shortHand: "pcl", adminOnly: true)]
    public void ChangeCombatBreakdownDetailLevel(ChatCommandContext ctx, string v)
    {
        try
        {
            Settings.CombatBreakdownDetail = int.Parse(v);
            ctx.Reply($"combat breakdown detail level has been set to: {v}");
        }
        catch (Exception)
        {
            ctx.Reply($"failed to parse value: {v}, using keeping current value at: {Settings.CombatBreakdownDetail}");
            ctx.Reply($"Make sure to use a number between 0 and 3.");
        }
    }

    [Command("pvpme", description: "prints the player's stats")]
    public void PvPMe(ChatCommandContext ctx)
    {
        if (!PlayerStatStore.PlayerStats.TryGetValue(ctx.User.PlatformId, out var s))
        {
            ctx.Reply("Oh no! You don't exist, silly!");
            return;
        }

        static string Label(string text) => text.Color(ChatAnnouncements.assistColor).Bold();
        static string Value(int number) => number.ToString().Color(ChatAnnouncements.lvlColor).Bold();

        string[] lines =
        {
        $"•••••••••••••••••••{ctx.Name}'s PvP Stats •••••••••••••••••••".Bold().Color(ChatAnnouncements.assistColor),
        $"{Label("Kills:")} {Value(s.Kills)}",
        $"{Label("Deaths:")} {Value(s.Deaths)}",
        $"{Label("Assists:")} {Value(s.Assists)}",
        $"{Label("Current Streak:")} {Value(s.CurrentKillStreak)}",
        $"{Label("Highest Streak:")} {Value(s.HighestKillStreak)}",
        $"{Label("Damage Dealt:")} {Value(s.Damage)}",
        $"{Label("Damage Taken:")} {Value(s.DamageTaken)}"
        };

        foreach (var line in lines)
        {
            ctx.Reply(line);
        }
    }

    //DEBUG COMMANDS:::===================================================================== 
    //DEBUG COMMANDS:::===================================================================== 
    //DEBUG COMMANDS:::===================================================================== 

    [Command("t_ak", adminOnly: true)]
    public void SelfTestCommand(ChatCommandContext ctx)
    {
        DebugTesting.Test_SendBasicKillMessage();
        ctx.Reply("===SendBasicKillMessage test sent!===");
    }

    //DEBUG COMMANDS:::===================================================================== 
    //DEBUG COMMANDS:::===================================================================== 
    //DEBUG COMMANDS:::===================================================================== 

}