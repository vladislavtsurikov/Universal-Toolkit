#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace VladislavTsurikov.Utility
{
    public static class FindDuplicateLocalizationIDs
    {
        [MenuItem("Tools/Localization/Find Duplicate Localization IDs")]
        public static void FindDuplicates()
        {
            string[] sharedTableGuids = AssetDatabase.FindAssets("t:SharedTableData");

            var globalIdMap = new Dictionary<long, List<string>>();
            bool hasDuplicates = false;

            foreach (var guid in sharedTableGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var sharedTableData = AssetDatabase.LoadAssetAtPath<SharedTableData>(path);

                if (sharedTableData == null)
                {
                    continue;
                }

                foreach (var entry in sharedTableData.Entries)
                {
                    if (!globalIdMap.ContainsKey(entry.Id))
                    {
                        globalIdMap[entry.Id] = new List<string>();
                    }

                    globalIdMap[entry.Id].Add($"{path} (key: {entry.Key})");
                }
            }

            foreach (var kvp in globalIdMap)
            {
                if (kvp.Value.Count > 1)
                {
                    hasDuplicates = true;
                    Debug.LogError($"Duplicate ID {kvp.Key} found in:\n{string.Join("\n", kvp.Value)}");
                }
            }

            if (!hasDuplicates)
            {
                Debug.Log("No duplicate localization IDs found.");
            }
        }
    }
}
#endif