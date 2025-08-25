﻿#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public class EnumArrayFieldDrawer : IMGUIFieldDrawer
    {
        public override bool CanDraw(Type fieldType)
        {
            return fieldType.IsArray && fieldType.GetElementType().BaseType == typeof(Enum);
        }

        public override object Draw(Rect rect, GUIContent label, Type fieldType, object value)
        {
            Type enumType = fieldType.GetElementType();
            Array enumArray = (Array)value ?? Array.CreateInstance(enumType, 0);
            
            int mask = 0;
            foreach (Enum enumValue in enumArray)
            {
                mask |= 1 << Convert.ToInt32(enumValue);
            }
            
            EditorGUI.BeginChangeCheck();
            mask = EditorGUI.MaskField(rect, label, mask, Enum.GetNames(enumType));
            
            if (EditorGUI.EndChangeCheck())
            {
                List<Enum> selectedValues = new List<Enum>();
                
                Array enumValues = Enum.GetValues(enumType);
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        selectedValues.Add((Enum)enumValues.GetValue(i));
                    }
                }
                
                Array newArray = Array.CreateInstance(enumType, selectedValues.Count);
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    newArray.SetValue(selectedValues[i], i);
                }
                
                return newArray;
            }
            
            return enumArray;
        }
    }
}
#endif