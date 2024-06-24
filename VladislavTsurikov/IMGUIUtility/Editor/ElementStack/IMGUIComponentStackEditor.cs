#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class IMGUIComponentStackEditor<T, N> : ComponentStackEditor<T, N>
        where T: Component
        where N: IMGUIElementEditor
    {
        public IMGUIComponentStackEditor(AdvancedComponentStack<T> stack) : base(stack)
        {
        }

        protected virtual void OnIMGUIComponentStackGUI()
        {
            for (int i = 0; i < Editors.Count; i++)
            {
                OnGUIElement(Editors[i], i);
            }
        }

        protected virtual void OnGUIElement(N editor, int index)
        {
            try
            {
                if (editor.GetType().GetAttribute<DontDrawFoldoutAttribute>() == null)
                {
                    editor.Target.SelectSettingsFoldout = CustomEditorGUILayout.HeaderWithMenu(editor.Target.Name, editor.Target.SelectSettingsFoldout,
                        () => Menu(index)
                    );
                
                    if(editor.Target.SelectSettingsFoldout)
                    {
                        EditorGUI.indentLevel++;

                        editor.OnGUI();

                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    editor.OnGUI();
                }
            }
            catch (Exception e)
            {
                Stack.Reset(index);
                throw;
            }
        }
        
        protected virtual void Menu(int index)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => Stack.Reset(index));

            menu.ShowAsContext();
        }

        public void OnGUI()
        {
            if(Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                Stack.IsDirty = false;
            }

            OnIMGUIComponentStackGUI();
        }

        public void DrawElements(List<Type> drawTypes)
        {
            foreach (var type in drawTypes)
            {
                DrawElement(type);
            }
        }

        public void DrawElement(Type type)
        {
            if(Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();  
                RefreshEditors();
                Stack.IsDirty = false;
            }
            
            for (int i = 0; i < Editors.Count; i++)
            {
                if(Editors[i].Target.GetType() == type)
                {
                    OnGUIElement(Editors[i], i);
                }
            }
        }
        
        public N GetEditor(Type type)
        {
            foreach (var editor in Editors)
            {
                if(editor.Target.GetType() == type)
                {
                    return editor;
                }
            }

            return null;
        }
    }
}
#endif