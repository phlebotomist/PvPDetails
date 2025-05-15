using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace PvPDetails;

[HarmonyPatch(typeof(VampireDownedServerEventSystem), nameof(VampireDownedServerEventSystem.OnUpdate))]
public static class VampireDownedHook
{
    public static void Prefix(VampireDownedServerEventSystem __instance)
    {
        var downedEvents = __instance.__query_1174204813_0.ToEntityArray(Allocator.Temp);
        foreach (var entity in downedEvents)
        {
            HandleDownedVampire(entity);
            CleanupVampireHits(entity);
        }
    }

    private static void CleanupVampireHits(Entity entity)
    {
        if (!VampireDownedServerEventSystem.TryFindRootOwner(entity, 1, Helpers.Server.EntityManager, out var ownerEntity))
        {
            Plugin.L.LogWarning("Failed to get entity on cleanup.");
            return;
        }
        var PlatformId = ownerEntity.Read<PlayerCharacter>().UserEntity.Read<User>().PlatformId;
        PlayerHitStore.ResetPlayerHitInteractions(PlatformId);
    }
    private static void HandleDownedVampire(Entity entity)
    {
        Helpers.P($"Vampire Downed: {entity}");
        if (!VampireDownedServerEventSystem.TryFindRootOwner(entity, 1, Helpers.Server.EntityManager, out var victimEntity))
        {
            Plugin.L.LogWarning("Failed to get victim entity");
            return;
        }

        if (!VampireDownedServerEventSystem.TryFindRootOwner(entity.Read<VampireDownedBuff>().Source, 1, Helpers.Server.EntityManager, out var killerEntity))
        {
            Plugin.L.LogWarning("Failed to get killer entity");
            return;
        }

        PlayerCharacter victim = victimEntity.Read<PlayerCharacter>();
        ulong victimId = victim.UserEntity.Read<User>().PlatformId;

        if (!killerEntity.Has<UnitLevel>() && !killerEntity.Has<PlayerCharacter>())
        {
            Plugin.L.LogError($"Killer could not be found: {killerEntity}");
            return;
        }

        if (killerEntity.Has<UnitLevel>())
        {
            PvPEventHandlers.OnPvEDeath();
            return;
        }

        PlayerCharacter killer = killerEntity.Read<PlayerCharacter>();
        ulong killerId = killer.UserEntity.Read<User>().PlatformId;

        if (killer.UserEntity == victim.UserEntity)
        {
            Plugin.L.LogInfo($"{victim.Name} killed himself"); //TODO: might be silver, handle as killsteal?
            return;
        }

        int killerLvl = Helpers.GetGearScore(killerEntity);
        int victimLvl = Helpers.GetGearScore(victimEntity);

        Dictionary<ulong, (string, int)> attackers = PlayerHitStore.GetRecentAttackersWithLvl(victimId);//filters out self hits by default
        if (attackers.TryGetValue(killerId, out (string, int) name_lvl))
        {
            killerLvl = name_lvl.Item2;
        }

        int victimPeakLvl = PlayerHitStore.GetHighestLvlUsedOnKiller(victimId, killerId);
        victimLvl = Math.Max(victimPeakLvl, victimLvl);
        PvPEventHandlers.OnPvPDeath(killerId, killer.Name.ToString(), killerLvl, victimId, victim.Name.ToString(),
            victimLvl, attackers.Keys.ToArray());
    }
}
