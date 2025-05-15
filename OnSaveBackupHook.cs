using HarmonyLib;
using ProjectM;


namespace PvPDetails;

// ProjectM.TriggerPersistenceSaveSystem.TriggerAutoSave
[HarmonyPatch(typeof(TriggerPersistenceSaveSystem), nameof(TriggerPersistenceSaveSystem.TriggerAutoSave))]
public static class TriggerPersistenceSaveSystemHook
{
    public static void Prefix(TriggerPersistenceSaveSystem __instance)
    {
        // We save data here becuase if server restarts or crashes we want the data to be based on last save
        PlayerStatStore.SaveData();
    }

}
