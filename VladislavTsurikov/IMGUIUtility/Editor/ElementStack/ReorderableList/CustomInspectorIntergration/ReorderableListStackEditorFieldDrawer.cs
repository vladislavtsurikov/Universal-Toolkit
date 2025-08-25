#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    public class ReorderableListStackEditorFieldDrawer : IMGUIFieldDrawer
    {
        private readonly Dictionary<object, object> _cachedEditors = new Dictionary<object, object>();

        public override bool CanDraw(Type type)
        {
            return type.TryGetGenericArgument(typeof(AdvancedComponentStack<>)) != null;
        }

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            if (value == null)
            {
                return null;
            }

            var collectionEditor = GetOrCreateEditor(value, fieldType, label);

            if (collectionEditor == null)
            {
                return null;
            }

            Rect listRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, rect.height - EditorGUIUtility.singleLineHeight);

            MethodInfo onGuiMethod = collectionEditor.GetType().GetMethod(
                "OnGUI",
                new[] { typeof(Rect) }
            );

            onGuiMethod?.Invoke(collectionEditor, new object[] { listRect });

            return value;
        }

        public override bool ShouldCreateInstanceIfNull()
        {
            return true;
        }

        public override float GetFieldsHeight(object target)
        {
            if (!_cachedEditors.TryGetValue(target, out var collectionEditor) || 
                collectionEditor.GetType().GetMethod("GetElementStackHeight") is not { } getHeightMethod)
            {
                Debug.LogWarning("GetFieldsHeight: Missing collectionEditor or GetElementStackHeight method.");
                return EditorGUIUtility.singleLineHeight * 5;
            }

            return (float)getHeightMethod.Invoke(collectionEditor, null);
        }

        private object GetOrCreateEditor(object value, Type fieldType, GUIContent label)
        {
            if (!_cachedEditors.TryGetValue(value, out var collectionEditor))
            {
                var componentType = fieldType.TryGetGenericArgument(typeof(ComponentStack<>));

                if (componentType == null)
                {
                    Debug.LogError($"GetOrCreateEditor: Unable to determine componentType for {fieldType.FullName}.");
                    return null;
                }

                Type editorType = typeof(ReorderableListStackEditor<,>).MakeGenericType(
                    componentType,
                    typeof(ReorderableListComponentEditor)
                );

                collectionEditor = Activator.CreateInstance(
                    editorType,
                    label,
                    value,
                    true
                );

                _cachedEditors[value] = collectionEditor;
            }

            return collectionEditor;
        }
    }
}
#endif