#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.UnityUtility.Editor;
using VladislavTsurikov.Utility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class TabComponentStackEditor<T, N> : ComponentStackEditor<T, N>
        where T: Component
        where N: IMGUIElementEditor
    {
        protected AdvancedComponentStack<T> AdvancedComponentStack => (AdvancedComponentStack<T>)Stack;

        protected readonly TabStackEditor _tabStackEditor;

        public TabComponentStackEditor(AdvancedComponentStack<T> stack) : base(stack)
        {
            _tabStackEditor = new TabStackEditor(stack.ReorderableElementList, true, false)
            {
                AddCallback = ShowAddManu,
                AddTabMenuCallback = TabMenu,
                HappenedMoveCallback = HappenedMove,
                TabWidthFromName = true,
            };
        }

        public virtual void OnTabStackGUI()
        {
            _tabStackEditor.OnGUI();
        }
        
        public void DrawSelectedSettings()
        {
            if(Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                Stack.IsDirty = false;
            }
            
            if(Stack.ElementList.Count == 0)
            {
                return;
            }

            OnSelectedComponentGUI();
        }

        protected virtual void OnSelectedComponentGUI()
        {
            if(Stack.SelectedElement == null)
            {
                return;
            }
            
            SelectedEditor?.OnGUI();
        }

        protected virtual GenericMenu TabMenu(int currentTabIndex)
        {
            GenericMenu menu = new GenericMenu();

            if (Stack.ElementList.Count > 1)
            {
                menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback, new Action(() => { Stack.Remove(currentTabIndex); }));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Delete"));
            }    

            return menu;
        }
        
        protected virtual void ShowAddManu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var item in AllEditorTypes<T>.Types)
            {
                Type settingsType = item.Key;
                
                if (settingsType.GetAttribute(typeof(DontCreateAttribute)) != null)
                {
                    continue;
                }
                
                if (settingsType.GetAttribute<PersistentComponentAttribute>() != null || settingsType.GetAttribute<DontShowInAddMenuAttribute>() != null) 
                {
                    continue;
                }
                
                string context = settingsType.GetAttribute<NameAttribute>().Name;

                if (Stack is ComponentStackSupportSameType<T> componentStackWithSameTypes)
                {
                    menu.AddItem(new GUIContent(context), false, () => componentStackWithSameTypes.CreateComponent(settingsType));
                }
                else if (Stack is ComponentStackOnlyDifferentTypes<T> componentStackWithDifferentTypes)
                {
                    bool exists = componentStackWithDifferentTypes.HasType(settingsType);

                    if (!exists)
                    {
                        menu.AddItem(new GUIContent(context), false, () => componentStackWithDifferentTypes.CreateIfMissingType(settingsType));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(context));
                    }
                }
            }

            menu.ShowAsContext();
        }

        private void HappenedMove()
        {
            Stack.IsDirty = true;
        }
    }
}
#endif