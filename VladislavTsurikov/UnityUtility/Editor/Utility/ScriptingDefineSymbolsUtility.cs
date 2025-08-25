#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace VladislavTsurikov.UnityUtility.Editor
{
    public static class ScriptingDefineSymbolsUtility
    {
        public static void SetScriptingDefineSymbols(string define)
        {
            var defineList = new List<string>(PlayerSettings
                .GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';'));
            if (!defineList.Contains(define))
            {
                defineList.Add(define);
                var defines = string.Join(";", defineList.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                    defines);
            }
        }
    }
}
#endif
