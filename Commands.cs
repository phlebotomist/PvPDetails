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
}