using VampireCommandFramework;
using VampireWebhook;
namespace PvPDetails;

public class Commands
{
    [Command("pvpdetails", shortHand: "pd_", description: "test loaded", adminOnly: true)]
    public void test(ChatCommandContext ctx)
    {
        ctx.Reply("loaded happy");
    }
    [Command("testhookdep", shortHand: "pd_dep", description: "test to make sure dep all set", adminOnly: true)]
    public void test2(ChatCommandContext ctx)
    {
        _ = DiscordWebhook.SendDiscordMessageAsync("test from pd ");
    }
}