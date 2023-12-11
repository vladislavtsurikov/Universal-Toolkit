#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.ColliderSystem.Runtime
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