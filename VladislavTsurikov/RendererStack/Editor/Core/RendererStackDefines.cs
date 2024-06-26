﻿#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.UnityUtility.Editor;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.RendererStack.Editor.Core
{
    [InitializeOnLoad]
    public class RendererStackDefines
    {
        private static readonly string _defineRendererStack = "RENDERER_STACK";

        static RendererStackDefines()
        {
            ScriptingDefineSymbolsUtility.SetScriptingDefineSymbols(_defineRendererStack);
        }
    }
}
#endif