using System;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Systems;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Entities;

namespace PvPDetails;


[HarmonyPatch(typeof(StatChangeSystem), nameof(StatChangeSystem.OnUpdate))]
public static class StatChangeHook
{
    public static void Prefix(StatChangeSystem __instance)
    {
        var mc = __instance._MergedChanges;
        foreach (var stEvent in mc)
        {
            if (stEvent.Reason != StatChangeReason.DealDamageSystem_0)
                continue;

            if (stEvent.Change >= 0)// healing, we don't care about it right now
                return;

            EntityManager em = __instance.EntityManager;
            if (!em.HasComponent<EntityOwner>(stEvent.Source))
                return;


            if (!em.HasComponent<PlayerCharacter>(stEvent.Entity))
                return;

            Entity defenderEntity = stEvent.Entity;
            PlayerCharacter defenderCharacter = em.GetComponentData<PlayerCharacter>(stEvent.Entity);
            Entity attackerEntity = em.GetComponentData<EntityOwner>(stEvent.Source).Owner;

            if (!em.HasComponent<PlayerCharacter>(attackerEntity))
                return;

            PlayerCharacter attackerCharacter = em.GetComponentData<PlayerCharacter>(attackerEntity);

            ulong attackerPlatformId = attackerCharacter.UserEntity.Read<User>().PlatformId;
            ulong victimPlatformId = defenderCharacter.UserEntity.Read<User>().PlatformId;
            string attackerName = attackerCharacter.Name.ToString();
            string victimName = defenderCharacter.Name.ToString();

            int attackerLvl = getGearScore(attackerEntity);
            int defenderLvl = getGearScore(defenderEntity);

            int abilityHash = GetAbilityGUIDHash(stEvent.Source);
            int hpChange = (int)Math.Round(stEvent.Change);
            int dmgAmount = Math.Abs(hpChange);

            PvPEventHandlers.OnPvPHit(attackerPlatformId, attackerName, attackerLvl, victimPlatformId, victimName,
                defenderLvl, abilityHash, dmgAmount);
        }
    }

    private static int GetAbilityGUIDHash(Entity dmgSource)
    {
        if (dmgSource != Entity.Null && Helpers.Server.EntityManager.HasComponent<PrefabGUID>(dmgSource))
        {
            return Helpers.Server.EntityManager.GetComponentData<PrefabGUID>(dmgSource).GuidHash;
        }
        return -1;
    }

    private static int getGearScore(Entity entity)
    {
        if (entity.Has<Equipment>())
        {
            var equipment = entity.Read<Equipment>();
            float gs = equipment.GetFullLevel();
            return (int)Math.Round(gs);
        }
        return -1;
    }
}
