#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Component;
using GUIUtility = VladislavTsurikov.Utility.Runtime.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group
{
    public class ClipboardGroup : ClipboardObject
    {
        private ClipboardStack<Component> _clipboardGroupGeneralComponentStack;
        private ClipboardStack<Component> _clipboardGroupComponentStack;

        public ClipboardGroup(Type toolType, Type prototypeType) : base(prototypeType, toolType)
        {
            _clipboardGroupGeneralComponentStack = new ClipboardStack<Component>();
            _clipboardGroupComponentStack = new ClipboardStack<Component>();
        }
        
        protected override void Copy(List<IHasElementStack> objects)
        {
            _clipboardGroupGeneralComponentStack.Copy(GetGeneralStacks(objects));
            _clipboardGroupComponentStack.Copy(GetStacks(objects));
        }

        protected override void ClipboardAction(List<IHasElementStack> objects, bool paste)
        {
            _clipboardGroupGeneralComponentStack.ClipboardAction(GetGeneralStacks(objects), paste);
            _clipboardGroupComponentStack.ClipboardAction(GetStacks(objects), paste);
        }

        protected override void ClipboardAction(List<IHasElementStack> objects, Type settingsType, bool paste)
        {
            _clipboardGroupGeneralComponentStack.ClipboardAction(GetGeneralStacks(objects), settingsType, paste);
            _clipboardGroupComponentStack.ClipboardAction(GetStacks(objects), settingsType, paste);
        }

        private List<Component> GetAllCopiedComponent()
        {
            List<Component> copiedComponents = (List<Component>)_clipboardGroupGeneralComponentStack.CopiedComponentList;
            copiedComponents.AddRange(_clipboardGroupComponentStack.CopiedComponentList);
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

        public void GroupMenu(GenericMenu menu, SelectedData selectedData)
        {
            if (AllowMenu(selectedData))
            {
                if(selectedData.HasOneSelectedGroup())
                {
                    menu.AddItem(new GUIContent("Copy All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => 
                        Copy(new List<IHasElementStack>{ selectedData.SelectedGroup })));
                }

                if(GetAllCopiedComponent().Count != 0)
                {
                    menu.AddItem(new GUIContent("Paste All Settings"), false, GUIUtility.ContextMenuCallback, new Action(() => 
                        ClipboardAction(new List<IHasElementStack>(selectedData.SelectedGroupList), true)));

                    foreach (Component component in GetAllCopiedComponent())
                    {
                        MenuItemAttribute menuItemAttribute = component.GetType().GetAttribute<MenuItemAttribute>();
                        
                        if (menuItemAttribute == null)
                        {
                            Debug.Log("MenuItem is not found for " + component.Name);
                            continue;
                        }
                        
                        menu.AddItem(new GUIContent("Paste Settings/" + menuItemAttribute.Name), false, GUIUtility.ContextMenuCallback, 
                            new Action(() => ClipboardAction(new List<IHasElementStack>(selectedData.SelectedGroupList), component.GetType(), true)));	
                    }
                }
            }
        }

        private bool AllowMenu(SelectedData selectedData)
        {
            if(selectedData.HasOneSelectedGroup())
            {
                if(selectedData.SelectedGroup.ComponentStackManager.GetAllElementTypes(ToolType, PrototypeType).Count == 0)
                {
                    return false;
                }
            }
            
            if(selectedData.HasOneSelectedGroup())
            {
                return true;
            }
            else if (GroupUtility.GetGeneralPrototypeType(selectedData.SelectedGroupList) != null)
            {
                return true;
            }

            return false;
        }
    }
}
#endif