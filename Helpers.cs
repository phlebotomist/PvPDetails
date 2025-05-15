using ProjectM;
using Unity.Collections;
using Unity.Entities;

namespace PvPDetails;

public static class Helpers
{
    public static World Server { get; } = GetWorld("Server");
    public static string Pad(string s, int n)
    {
        return new string(' ', n) + s + new string(' ', n);
    }

    public static string PadR(string s, int n)
    {
        return s + new string(' ', n);
    }
    public static string PadL(string s, int n)
    {
        return new string(' ', n) + s;
    }

    public static void P(string s)
    {
        var FixedString = new FixedString512Bytes(s);
        ServerChatUtils.SendSystemMessageToAllClients(Server.EntityManager, ref FixedString);
    }
    private static World GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name == name)
            {
                return world;
            }
        }
        return null;
    }

}