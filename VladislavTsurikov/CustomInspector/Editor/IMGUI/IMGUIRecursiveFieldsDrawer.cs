#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class IMGUIRecursiveFieldsDrawer : RecursiveFieldsDrawer
    {
        public float DrawRecursiveFields(
            object value,
            FieldInfo fieldInfo,
            Rect fieldRect,
            Action<object, Rect> drawField)
        {
            EditorGUI.indentLevel++;
                    
            bool foldoutState = GetFoldoutState(value);

            foldoutState = EditorGUI.Foldout(fieldRect, foldoutState, ObjectNames.NicifyVariableName(fieldInfo.Name));

            SetFoldoutState(value, foldoutState);
            
            float foldoutHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            fieldRect.y += foldoutHeight;

            if (foldoutState)
            {
                drawField(value, fieldRect);
            }
            
            EditorGUI.indentLevel--;

            return foldoutHeight;
        }
    }
}
#endif