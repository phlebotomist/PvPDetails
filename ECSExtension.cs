using System;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime;
using ProjectM;
using Stunlock.Core;
using Unity.Entities;

namespace PvPDetails;

#pragma warning disable CS8500
public static class ECSExtensions
{
    public unsafe static T Read<T>(this Entity entity) where T : struct
    {
        var ct = new ComponentType(Il2CppType.Of<T>());
        void* rawPointer = Helpers.Server.EntityManager.GetComponentDataRawRO(entity, ct.TypeIndex);
        T componentData = Marshal.PtrToStructure<T>(new IntPtr(rawPointer));

        return componentData;
    }
    public static bool Has<T>(this Entity entity)
    {
        var ct = new ComponentType(Il2CppType.Of<T>());
        return Helpers.Server.EntityManager.HasComponent(entity, ct);
    }
}
#pragma warning restore CS8500
