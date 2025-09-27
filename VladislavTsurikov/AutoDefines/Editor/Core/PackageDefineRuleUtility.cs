#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace VladislavTsurikov.AutoDefines.Editor.Core
{
    public static class PackageDefineRuleUtility
    {
        private static readonly Dictionary<NamedBuildTarget, HashSet<string>> _pendingDefines = new();
        private static bool _scheduled;

        public static void ApplyDefine(string defineSymbol, bool enable)
        {
            Array values = Enum.GetValues(typeof(NamedBuildTarget));
            for (var i = 0; i < values.Length; i++)
            {
                var nbt = (NamedBuildTarget)values.GetValue(i);
                if (nbt == NamedBuildTarget.Unknown)
                {
                    continue;
                }

                if (!_pendingDefines.TryGetValue(nbt, out HashSet<string> set))
                {
                    var current = PlayerSettings.GetScriptingDefineSymbols(nbt);
                    set = new HashSet<string>(current.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)));
                    _pendingDefines[nbt] = set;
                }

                if (enable)
                {
                    set.Add(defineSymbol);
                }
                else
                {
                    set.Remove(defineSymbol);
                }
            }

            if (!_scheduled)
            {
                _scheduled = true;
                EditorApplication.update += Flush;
            }
        }

        private static void Flush()
        {
            EditorApplication.update -= Flush;
            foreach (KeyValuePair<NamedBuildTarget, HashSet<string>> kv in _pendingDefines)
            {
                var joined = string.Join(";", kv.Value);
                PlayerSettings.SetScriptingDefineSymbols(kv.Key, joined);
            }

            _pendingDefines.Clear();
            _scheduled = false;
        }
    }
}
#endif
