#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class UnityObjectFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            return EditorGUI.ObjectField(rect, label, (UnityEngine.Object)value, fieldType, true);
        }

        public override bool ShouldCreateInstanceIfNull()
        {
            return false;
        }
    }
}
#endif