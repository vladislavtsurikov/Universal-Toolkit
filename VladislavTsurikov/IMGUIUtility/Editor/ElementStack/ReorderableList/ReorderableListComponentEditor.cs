#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Editor.Core;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    public class ReorderableListComponentEditor : ElementEditor
    {
        public virtual void OnGUI(Rect rect, int index) {}
        public virtual float GetElementHeight(int index) => EditorGUIUtility.singleLineHeight * 2;
    }
}
#endif