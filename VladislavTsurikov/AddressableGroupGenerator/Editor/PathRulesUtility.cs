#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;

namespace VladislavTsurikov.AddressableGroupGenerator.Editor
{
    public static class PathRulesUtility
    {
        public static readonly HashSet<string> ExcludedFolders = new()
        {
            "Resources",
            "Editor",
            "Gizmos",
            "StreamingAssets"
        };

        public static int GetFolderDepth(string path, string root)
        {
            string relative = path.Replace(root, "").Trim(Path.DirectorySeparatorChar);
            return relative.Split(Path.DirectorySeparatorChar).Length;
        }
    }
}
#endif