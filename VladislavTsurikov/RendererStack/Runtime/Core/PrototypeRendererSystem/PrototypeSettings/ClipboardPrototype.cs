#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;
using GUIUtility = VladislavTsurikov.Utility.Runtime.GUIUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    public class ClipboardPrototype 
    {
        private ClipboardStack<PrototypeComponent> _clipboardStack = new ClipboardStack<PrototypeComponent>();
        
        public void PrototypeMenu(GenericMenu menu, SelectedData selectedData)
        {
            if(selectedData.HasOneSelectedProto() || _clipboardStack.CopiedComponentList.Count != 0)
            {
                menu.AddSeparator("");
            }
            
			if(selectedData.HasOneSelectedProto())
			{
				menu.AddItem(new GUIContent("Copy All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => _clipboardStack.Copy(GetStacks(selectedData.SelectedProtoList))));
			}

            if(_clipboardStack.CopiedComponentList.Count != 0)
            {
                menu.AddItem(new GUIContent("Paste All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => _clipboardStack.ClipboardAction(GetStacks(selectedData.SelectedProtoList), true)));

                foreach (PrototypeComponent baseSettings in _clipboardStack.CopiedComponentList)
                {
                    string name = baseSettings.GetType().GetAttribute<MenuItemAttribute>().Name;
                    menu.AddItem(new GUIContent("Paste Settings/" + name), false, GUIUtility.ContextMenuCallback, new Action(() => _clipboardStack.ClipboardAction(GetStacks(selectedData.SelectedProtoList), baseSettings.GetType(), true)));	
                }
            }
        }
        
        private List<AdvancedComponentStack<PrototypeComponent>> GetStacks(List<Prototype> objects)
        {
            List<AdvancedComponentStack<PrototypeComponent>> stacks = new List<AdvancedComponentStack<PrototypeComponent>>();

            foreach (var obj in objects)
            {
                stacks.Add(obj.PrototypeComponentStack);
            }
            
            return stacks;
        }
    }
}
#endif