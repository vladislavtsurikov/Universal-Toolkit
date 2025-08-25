#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class IMGUIInspectorFieldsDrawer : InspectorFieldsDrawer<IMGUIFieldDrawer>
    {
        private readonly IMGUIRecursiveFieldsDrawer _imguiRecursiveFieldsDrawer = new IMGUIRecursiveFieldsDrawer();
        
        private float _totalHeight = 0;
        
        public IMGUIInspectorFieldsDrawer(
            List<Type> excludedDeclaringTypes = null,
            bool excludeInternal = true,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            : base(excludedDeclaringTypes, excludeInternal, bindingFlags) { }

        public void DrawFields(object target, Rect rect)
        {
            if (target == null)
            {
                EditorGUI.LabelField(rect, "Target is null");
                return;
            }

            _totalHeight = 0;
            
            DrawFieldsRecursive(target, rect);
        }

        private void DrawFieldsRecursive(object target, Rect rect)
        {
            if (target == null)
            {
                return;
            }

            foreach (var (drawer, field, fieldName, value) in GetProcessedFields(target))
            {
                GUIContent fieldLabel = new GUIContent(fieldName);

                if (drawer != null)
                {
                    float fieldHeight = drawer.GetFieldsHeight(value);
                    Rect fieldRect = EditorGUI.IndentedRect(new Rect(rect.x, rect.y, rect.width, fieldHeight));

                    EditorGUI.BeginChangeCheck();

                    object newValue = drawer.Draw(fieldRect, fieldLabel, field.FieldType, value);

                    if (EditorGUI.EndChangeCheck())
                    {
                        field.SetValue(target, newValue);
                    }

                    rect.y += fieldHeight;
                    _totalHeight += fieldHeight;
                }
                else
                {
                    float recursiveFieldsHeight =
                        _imguiRecursiveFieldsDrawer.DrawRecursiveFields(value, field, rect, DrawFieldsRecursive);

                    rect.y += recursiveFieldsHeight;
                    _totalHeight += recursiveFieldsHeight;
                }
            }
        }
        
        public float GetFieldsHeight(object target)
        {
            if (target == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return _totalHeight;
        }
    }
}
#endif