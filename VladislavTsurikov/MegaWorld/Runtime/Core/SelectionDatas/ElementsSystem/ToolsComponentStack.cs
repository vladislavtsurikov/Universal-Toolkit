﻿using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Attributes;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using Component = VladislavTsurikov.ComponentStack.Runtime.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    [Serializable]
    public class ToolsComponentStack : ComponentStackSupportSameType<ToolComponentStack>
    {
        [OdinSerialize]
        private Type _addElementsAttributeType;
        [OdinSerialize]
        private Type _prototypeType;

        protected override void OnSetup()
        {
            _addElementsAttributeType = (Type)InitializationDataForElements[0];
            _prototypeType = (Type)InitializationDataForElements[1];
        }
        
        protected override void OnCreateElements()
        {
            foreach (var type in AllToolTypes.TypeList)
            {
                ToolComponentStack toolComponentStack = GetToolStackElement(type);
                
                if (toolComponentStack == null)
                {
                    toolComponentStack = Create(typeof(ToolComponentStack));
                    toolComponentStack.ToolType = type;
                }
                
                foreach (var attribute in type.GetAttributes(_addElementsAttributeType))
                {
                    AddComponentsAttribute addComponentsAttribute = (AddComponentsAttribute)attribute;
                        
                    if (!addComponentsAttribute.PrototypeTypes.Contains(_prototypeType))
                    {
                        continue;
                    }

                    toolComponentStack.ComponentStack.CreateIfMissingType(addComponentsAttribute.Types);
                }
            }
        }
        
        public override void OnRemoveInvalidElements()
        {
            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                if (!ElementList[i].IsValid())
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
                    if (!IsElementBelongsToTool(toolComponentStack.ComponentStack.ElementList[i], toolType))
                    {
                        Remove(i);
                    }
                }
            }
        }

        private bool IsElementBelongsToTool(Element element, Type toolType)
        {
            foreach (var attribute in toolType.GetAttributes(_addElementsAttributeType))
            {
                AddComponentsAttribute addComponentsAttribute = (AddComponentsAttribute)attribute;

                if (!addComponentsAttribute.PrototypeTypes.Contains(_prototypeType))
                {
                    continue;
                }

                if (addComponentsAttribute.Types.Contains(element.GetType()))
                {
                    return true;
                }
            }

            return false;
        }
        
        public Component GetElement(Type toolType, Type typeElement)
        {
            foreach (var globalToolSettings in ElementList)
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

        public ToolComponentStack GetToolStackElement(Type toolType)
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
    }
}