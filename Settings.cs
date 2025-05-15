using BepInEx.Configuration;

namespace PvPDetails;

internal class Settings
{

    internal static int CombatBreakdownDetail { get; set; } = 2;
    internal static void Initialize(ConfigFile config)
    {
        CombatBreakdownDetail = config.Bind("General", "CombatBreakdownDetail", 2, "The level of detail you want to show in the combat report sent to discord.").Value;
    }
}
