#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class SpriteFieldDrawer : IMGUIFieldDrawer
    {
        private const float ObjectFieldSize = 50f;
        
        public override bool CanDraw(Type type)
        {
            return typeof(Sprite).IsAssignableFrom(type);
        }

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            float labelHeight = EditorGUIUtility.singleLineHeight;
            Rect labelRect = new Rect(rect.x, rect.y, labelWidth, labelHeight);
            Rect fieldRect = new Rect(rect.x + labelWidth, rect.y, ObjectFieldSize, ObjectFieldSize);

            EditorGUI.LabelField(labelRect, label);

            return EditorGUI.ObjectField(fieldRect, (Sprite)value, typeof(Sprite), true); 
        }
        
        public override bool ShouldCreateInstanceIfNull()
        {
            return false;
        }
        
        public override float GetFieldsHeight(object target)
        {
            return 50;
        }
    }
}
#endif