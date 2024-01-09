using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Components;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem.Elements
{
    [ElementEditor(typeof(ExpandFilter))]
    public class ExpandFilterEditor : MaskFilterEditor
    {
        private ExpandFilter _filter;

        public override void OnEnable()
        {
            _filter = (ExpandFilter)Target;
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