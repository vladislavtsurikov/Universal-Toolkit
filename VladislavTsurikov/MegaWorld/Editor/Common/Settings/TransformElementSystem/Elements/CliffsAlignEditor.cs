#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Components;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem.Elements
{
    [ElementEditor(typeof(CliffsAlign))]
    public class CliffsAlignEditor : ReorderableListComponentEditor
    {
        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif