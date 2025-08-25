#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class Vector3FieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type fieldType)
        {
            return fieldType == typeof(Vector3);
        }
    
        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            return EditorGUI.Vector3Field(rect, label, (Vector3)value);
        }
    }
}
#endif