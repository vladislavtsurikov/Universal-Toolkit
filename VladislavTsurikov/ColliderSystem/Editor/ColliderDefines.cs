#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.ColliderSystem.Editor
{
    [InitializeOnLoad]
    public class ColliderDefines
    {
        private static readonly string DEFINE_COLLIDER = "COLLIDER";

        static ColliderDefines()
        {
            ScriptingDefineSymbolsUtility.SetScriptingDefineSymbols(DEFINE_COLLIDER);
        }
    }
}
#endif