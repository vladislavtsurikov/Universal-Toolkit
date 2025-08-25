#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace VladislavTsurikov.AddressableGroupGenerator.Editor
{
    public static class AssetEntryUtility
    {
        public static bool ShouldSkipAsset(string path)
        {
            if (path.EndsWith(".meta") ||
                path.EndsWith(".cs") ||
                path.EndsWith(".asmdef") ||
                path.EndsWith(".unity"))
            {
                return true;
            }

            string normalizedPath = path.Replace('\\', '/');
            foreach (var excluded in PathRulesUtility.ExcludedFolders)
            {
                if (normalizedPath.Contains($"/{excluded}/"))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GenerateAddress(string path)
        {
            string contentRoot = ContentFolderUtility.GetContentRoot();
            string address = path.Replace($"{contentRoot}/", "");

            int addressableConfigIndex = address.IndexOf("AddressableConfigs", StringComparison.Ordinal);
            if (addressableConfigIndex >= 0)
            {
                int startIndex = addressableConfigIndex + "AddressableConfigs".Length;

                if (startIndex < address.Length && address[startIndex] == '/')
                {
                    startIndex++;
                }

                if (startIndex < address.Length)
                {
                    address = address.Substring(startIndex);
                }
                else
                {
                    address = string.Empty;
                }
            }

            string extension = Path.GetExtension(address);
            if (!string.IsNullOrEmpty(extension))
            {
                address = address.Substring(0, address.Length - extension.Length);
            }

            return address;
        }

        public static void AddAssetsToGroup(string folderPath, AddressableAssetGroup group, string label, bool filterByExtension, string extensionFilter)
        {
            string[] guids = AssetDatabase.FindAssets("t:Object", new[] { folderPath });
            var entryCreator = new FastAddressableEntryCreator(group);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (ShouldSkipAsset(path) || IsFolder(path))
                {
                    continue;
                }

                if (filterByExtension && Path.GetExtension(path) != extensionFilter)
                {
                    continue;
                }

                string address = GenerateAddress(path);
                entryCreator.CreateAndAddEntry(guid, address, label);
            }
        }
        
        private static bool IsFolder(string path)
        {
            return string.IsNullOrEmpty(Path.GetExtension(path)) && Directory.Exists(path);
        }
    }
}
#endif