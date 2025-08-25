#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.AddressableLabelMap;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.AddressableLabelMap
{
    public static class AddressableLabelMapGenerator
    {
        [MenuItem("Tools/Addressables/Generate Label Map")]
        public static void Generate()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var map = new Dictionary<Type, Dictionary<string, string>>();

            foreach (var group in settings.groups)
            {
                foreach (var entry in group.entries)
                {
                    var asset = entry.MainAsset;
                    if (asset == null)
                    {
                        continue;
                    }

                    var type = asset.GetType();
                    if (!map.TryGetValue(type, out var dict))
                    {
                        map[type] = dict = new();
                    }

                    dict[entry.address] = group.name;
                }
            }

            AddressableLabelMapAsset.Instance.SetMap(map);
            AssetDatabase.SaveAssets();

            Debug.Log("✅ AddressableLabelMap.asset updated");
        }
    }
}
#endif