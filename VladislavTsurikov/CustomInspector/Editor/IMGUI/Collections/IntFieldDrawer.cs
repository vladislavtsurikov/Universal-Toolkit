#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class IntFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type type)
        {
            return type == typeof(int);
        }

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            return EditorGUI.IntField(rect, label, value != null ? (int)value : 0);
        }
    }
}
#endif