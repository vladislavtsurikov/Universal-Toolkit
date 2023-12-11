#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using Component = VladislavTsurikov.ComponentStack.Runtime.Component;
using GUIUtility = VladislavTsurikov.Utility.Runtime.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes
{
    public class ClipboardPrototype : ClipboardObject
    {
        private ClipboardStack<Component> _clipboardPrototypeGeneralComponentStack;
        private ClipboardStack<Component> _clipboardPrototypeComponentStack; 
        
        public ClipboardPrototype(Type toolType, Type prototypeType) : base(prototypeType, toolType)
        {
            _clipboardPrototypeGeneralComponentStack = new ClipboardStack<Component>();
            _clipboardPrototypeComponentStack = new ClipboardStack<Component>();
        }
        
        protected override void Copy(List<IHasElementStack> objects)
        {
            _clipboardPrototypeGeneralComponentStack.Copy(GetGeneralStacks(objects));
            _clipboardPrototypeComponentStack.Copy(GetStacks(objects));
        }

        protected override void ClipboardAction(List<IHasElementStack> objects, bool paste)
        {
            _clipboardPrototypeGeneralComponentStack.ClipboardAction(GetGeneralStacks(objects), paste);
            _clipboardPrototypeComponentStack.ClipboardAction(GetStacks(objects), paste);
        }

        protected override void ClipboardAction(List<IHasElementStack> objects, Type settingsType, bool paste)
        {
            _clipboardPrototypeGeneralComponentStack.ClipboardAction(GetGeneralStacks(objects), settingsType, paste);
            _clipboardPrototypeComponentStack.ClipboardAction(GetStacks(objects), settingsType, paste);
        }

        private List<Component> GetAllCopiedComponent()
        {
            List<Component> copiedComponents = (List<Component>)_clipboardPrototypeGeneralComponentStack.CopiedComponentList;
            copiedComponents.AddRange(_clipboardPrototypeComponentStack.CopiedComponentList);
            return copiedComponents;
        }

        private List<AdvancedComponentStack<Component>> GetGeneralStacks(List<IHasElementStack> objects)
        {
            List<AdvancedComponentStack<Component>> stacks = new List<AdvancedComponentStack<Component>>();

            foreach (var obj in objects)
            {
                stacks.Add(obj.ComponentStackManager.GeneralComponentStack);
            }
            
            return stacks;
        }
        
        private List<AdvancedComponentStack<Component>> GetStacks(List<IHasElementStack> objects)
        {
            List<AdvancedComponentStack<Component>> stacks = new List<AdvancedComponentStack<Component>>();

            foreach (var obj in objects)
            {
                foreach (var toolComponentStack in obj.ComponentStackManager.ToolsComponentStack.ElementList)
                {
                    if (toolComponentStack.ToolType == ToolType)
                    {
                        stacks.Add(toolComponentStack.ComponentStack);
                    }
                }
            }
            
            return stacks;
        }
        
        public void PrototypeMenu(GenericMenu menu, SelectedData selectedData)
        {
            if (AllowMenu(selectedData))
            {
                menu.AddSeparator("");
                
                if(selectedData.HasOneSelectedPrototype())
                {
                    menu.AddItem(new GUIContent("Copy All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => 
                        Copy(new List<IHasElementStack> { selectedData.SelectedPrototype })));
                }
            
                if(GetAllCopiedComponent().Count != 0)
                {
                    menu.AddItem(new GUIContent("Paste All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => 
                        ClipboardAction(new List<IHasElementStack>(selectedData.SelectedPrototypeList), true)));

                    foreach (Component component in GetAllCopiedComponent())
                    {
                        MenuItemAttribute menuItemAttribute = component.GetType().GetAttribute<MenuItemAttribute>();
                        
                        if (menuItemAttribute == null)
                        {
                            Debug.Log("MenuItem is not found for " + component.Name);
                            continue;
                        }
                    
                        menu.AddItem(new GUIContent("Paste Settings/" + menuItemAttribute.Name), false, GUIUtility.ContextMenuCallback, 
                            new Action(() => ClipboardAction(new List<IHasElementStack>(selectedData.SelectedPrototypeList), component.GetType(), true)));	
                    }
                }
            }
        }
        
        private bool AllowMenu(SelectedData selectedData)
        {
            if(selectedData.HasOneSelectedPrototype())
            {
                if(selectedData.SelectedPrototype.ComponentStackManager.GetAllElementTypes(ToolType, PrototypeType).Count == 0)
                {
                    return false;
                }
            }

            if (selectedData.HasOneSelectedPrototype() || GetAllCopiedComponent().Count != 0)
            {
                return true;
            }

            return false;
        }
    }
}
#endif