// using HarmonyLib;
// using ProjectM;
// using ProjectM.Gameplay.Systems;
// using Unity.Entities;

// namespace PvPDetails;


// [HarmonyPatch(typeof(StatChangeSystem), nameof(StatChangeSystem.OnUpdate))]
// public static class StatChangeHook
// {
//     public static void Prefix(StatChangeSystem __instance)
//     {
//         var mc = __instance._MergedChanges;
//         foreach (var stEvent in mc)
//         {
//             if (stEvent.Reason != StatChangeReason.DealDamageSystem_0)
//                 continue;

//             Helpers.P($"StatChangeSystem change");

//             // Helpers.P($"StatChangeSystem change: {stEvent.Change}");
//         }

//     }
// }
