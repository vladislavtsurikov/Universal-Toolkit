﻿using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem
{
    public class ToolsComponentStack : ComponentStackSupportSameType<ToolComponentStack>
    {
        protected override void OnCreateElements()
        {
            foreach (var type in AllToolTypes.TypeList)
            {
                AddToolComponentsAttribute addToolComponentsAttribute = type.GetAttribute<AddToolComponentsAttribute>();

                if (addToolComponentsAttribute == null)
                {
                    continue;
                }  

                ToolComponentStack toolComponentStack = GetToolStackElement(type);
                
                if (toolComponentStack == null)
                {
                    toolComponentStack = Create(typeof(ToolComponentStack));
                    toolComponentStack.ToolType = type;
                }

                foreach (var globalSettingsType in addToolComponentsAttribute.Types)
                {
                    toolComponentStack.ComponentStack.CreateIfMissingType(globalSettingsType);
                }
            }
        }
        
        public override void OnRemoveInvalidElements()
        {
            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                if (!ElementList[i].DeleteElement())
                {
                    Remove(i);
                }
            }
            
            foreach (var toolType in AllToolTypes.TypeList)
            {
                ToolComponentStack toolComponentStack = GetToolStackElement(toolType);

                if (toolComponentStack == null)
                {
                    continue;
                }
                    
                for (int i = toolComponentStack.ComponentStack.ElementList.Count - 1; i >= 0; i--)
                {
                    AddToolComponentsAttribute addComponentsAttribute = toolType.GetAttribute<AddToolComponentsAttribute>();

                    if (addComponentsAttribute == null || toolComponentStack.ComponentStack.ElementList[i] == null)
                    {
                        toolComponentStack.ComponentStack.Remove(i);
                        continue;
                    }

                    if (!addComponentsAttribute.Types.Contains(toolComponentStack.ComponentStack.ElementList[i].GetType()))
                    {
                        toolComponentStack.ComponentStack.Remove(i);
                    }
                }
            }
        }

        public static Component GetElement(Type toolType, Type typeElement)
        {
            foreach (var globalToolSettings in GlobalSettings.Instance.ToolsComponentStack.ElementList)
            {
                if (globalToolSettings.ToolType == toolType)
                {
                    foreach (var element in globalToolSettings.ComponentStack.ElementList)
                    {
                        if (element.GetType() == typeElement)
                        {
                            return element;
                        }
                    }
                }
            }
            
            return null;
        }

        private ToolComponentStack GetToolStackElement(Type toolType)
        {
            foreach (var element in _elementList)
            {
                if (element.ToolType == toolType)
                {
                    return element;
                }
            }

            return null;
        }
        
        public void Reset(Type toolType)
        {
            foreach (var item in ElementList)
            {
                if (item.ToolType == toolType)
                {
                    item.ComponentStack.Reset();
                }
            }
        }
    }
}