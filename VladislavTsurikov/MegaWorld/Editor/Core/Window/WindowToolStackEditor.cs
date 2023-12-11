#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public class WindowToolStackEditor : TabComponentStackEditor<ToolWindow, ToolWindowEditor>
    {
        private readonly WindowToolStack _windowToolStack;
        
        public WindowToolStackEditor(WindowToolStack stack) : base(stack)
        {
            _windowToolStack = stack;
        }

        public override void OnTabStackGUI()
        {
            _tabStackEditor.OnGUI();

            WindowData.Instance.SelectedTool = Stack.SelectedElement;

            if(WindowData.Instance.SelectedTool == null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("No Tool Selected");
                EditorGUILayout.EndVertical();
            }
        }

        public void DrawSelectedEditor()
        {
            if(WindowData.Instance.SelectedTool == null)
			{
				return;
			}

            for (int i = 0; i < Stack.ElementList.Count; i++)
            {
                if(Stack.ElementList[i].Selected)
                {
                    Stack.ElementList[i].HandleKeyboardEventsInternal();
                    Editors[i].OnGUI();
                    break;
                }
            }
        }
    }
}
#endif