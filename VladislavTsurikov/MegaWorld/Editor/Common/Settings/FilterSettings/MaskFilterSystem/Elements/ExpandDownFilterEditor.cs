#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Components;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem.Elements
{
    [ElementEditor(typeof(ExpandDownFilter))]
    public class ExpandDownFilterEditor : MaskFilterEditor
    {
        private ExpandDownFilter _filter;

        public override void OnEnable()
        {
            _filter = (ExpandDownFilter)Target;
        }

        public override void OnGUI(Rect rect, int index) 
        {
            _filter.KernelSize = Mathf.Max(0.1f, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), KernelSize, _filter.KernelSize));
        }

        public override float GetElementHeight(int index) 
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }

        public static readonly GUIContent KernelSize = EditorGUIUtility.TrTextContent("Kernel Size");
    }
}
#endif