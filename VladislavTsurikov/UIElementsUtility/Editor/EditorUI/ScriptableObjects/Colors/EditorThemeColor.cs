#if UNITY_EDITOR
using System;
using UnityEditor;
using VladislavTsurikov.UIElementsUtility.Runtime.Color;

namespace VladislavTsurikov.UIElementsUtility.Editor.EditorUI.ScriptableObjects.Colors
{
    [Serializable]
    public class EditorThemeColor : ThemeColor
    {
        public override bool IsDarkTheme => EditorGUIUtility.isProSkin;
    }
}
#endif
