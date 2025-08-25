#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class StringFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type)
        {
            return type == typeof(string);
        }

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            return EditorGUI.TextField(rect, label, value as string);
        }
    }
}
#endif