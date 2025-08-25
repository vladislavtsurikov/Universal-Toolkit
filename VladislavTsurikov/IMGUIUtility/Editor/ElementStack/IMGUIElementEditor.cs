#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class IMGUIElementEditor : ElementEditor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new(
            excludedDeclaringTypes: new System.Collections.Generic.List<Type> { typeof(Runtime_Core_Component), typeof(Element) }
        );
        
        public virtual void OnGUI()
        {
            if (Target == null)
            {
                EditorGUILayout.LabelField("No target assigned.");
                return;
            }
            
            float totalHeight = _fieldsDrawer.GetFieldsHeight(Target);

            Rect rect = EditorGUILayout.GetControlRect(false, totalHeight);
            _fieldsDrawer.DrawFields(Target, rect);
        }
    }
}
#endif