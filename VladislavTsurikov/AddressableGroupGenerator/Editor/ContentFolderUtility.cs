#if UNITY_EDITOR
using System.IO;

namespace VladislavTsurikov.AddressableGroupGenerator.Editor
{
    public static class ContentFolderUtility
    {
        private const string ContentRoot = "Assets/Content";

        public static string GetContentRoot()
        {
            return ContentRoot;
        }

        public static string GetGroupName(string folderPath)
        {
            return folderPath.Replace($"{ContentRoot}/", "").Replace("/", "_");
        }
        
        public static bool IsValidContentFolder(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return false;

            string normalizedPath = path.Replace('\\', '/');
            foreach (var excluded in PathRulesUtility.ExcludedFolders)
            {
                if (normalizedPath.Contains($"/{excluded}/") ||
                    normalizedPath.EndsWith($"/{excluded}"))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
#endif