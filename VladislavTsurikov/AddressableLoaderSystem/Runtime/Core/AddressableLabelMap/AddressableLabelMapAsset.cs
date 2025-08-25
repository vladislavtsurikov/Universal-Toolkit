using System;
using System.Collections.Generic;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;
using OdinSerializer;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.AddressableLabelMap
{
    [LocationAsset("AddressableLoaderSystem/AddressableLabelMap")]
    public class AddressableLabelMapAsset : SerializedScriptableObjectSingleton<AddressableLabelMapAsset>
    {
        [OdinSerialize]
        private Dictionary<Type, Dictionary<string, string>> _map = new();
        
        private static List<string> _cachedLabels = null;

        public bool TryGetLabel(string address, out string label)
        {
            foreach (var entry in _map.Values)
            {
                if (entry.TryGetValue(address, out label))
                    return true;
            }

            label = null;
            return false;
        }

        public Dictionary<string, string> GetLabelsByType(Type type)
        {
            return _map.TryGetValue(type, out var result) ? result : new();
        }
        
        public List<string> GetAllLabels()
        {
            if (_cachedLabels != null)
            {
                return _cachedLabels;
            }

            var allLabels = new HashSet<string>();

            foreach (var typeMap in _map.Values)
            {
                foreach (var label in typeMap.Values)
                {
                    allLabels.Add(label);
                }
            }

            _cachedLabels = new List<string>(allLabels);
            return _cachedLabels;
        }

#if UNITY_EDITOR
        public void SetMap(Dictionary<Type, Dictionary<string, string>> newMap)
        {
            _map = newMap;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}