using System.Collections.Generic;


namespace PvPDetails
{
    /// <summary>
    /// Resolves prefab GUIDs to human-readable ability/weapon names.
    /// </summary>
    public static class HitNameResolver
    {
        private static readonly Dictionary<int, string> _guidToName = new()
        {
            // maybe we should load these in from a file so admins can make custom names? TODO: load these from a file
            { 1998252380, "Shadowbolt"},
            {706730253, "Frenzy"},
            {-1733898626, "Axe melee 1"},
            {-1192587580, "Axe melee 2"},
            {-1064937884, "Axe melee 3"},
            {705543074, "X-Strike"},
            {1784649791,"X-Strike"},
            {-1274932233, "ExplosiveShot"},
            {728144325 ,"ExplosiveShot explosion"},
            {853298599, "silver sickness"}
        };

        /// <summary>
        /// Returns the name for the given prefab GUID, or "<GUID>" if not found.
        /// </summary>
        public static string Resolve(int prefabGuid)
        {
            return _guidToName.TryGetValue(prefabGuid, out var name)
                ? name
                : $"<{prefabGuid}>";
        }
    }
}
