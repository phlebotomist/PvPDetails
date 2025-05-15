using VampireCommandFramework;
using VampireWebhook;
namespace PvPDetails;

public class Commands
{
    [Command("pvpdetails", shortHand: "pd_", description: "test plugin is loaded loaded", adminOnly: true)]
    public void testpvpdetails(ChatCommandContext ctx)
    {
        ctx.Reply("pvpdetails loaded happily!");
    }
    [Command("pvpme", description: "prints the players stats", adminOnly: true)]
    public void PvPMe(ChatCommandContext ctx)
    {
        ctx.Reply($"here's your stats {ctx.Name}! {ctx.User.PlatformId}");
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