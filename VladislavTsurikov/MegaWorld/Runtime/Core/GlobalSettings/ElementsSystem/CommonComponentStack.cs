﻿using System;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem.Attributes;
using Component = VladislavTsurikov.ComponentStack.Runtime.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem
{
    [Serializable]
    public class CommonComponentStack : ComponentStackOnlyDifferentTypes<Component>
    {
        protected override void OnCreateElements()
        {
            foreach (var type in AllToolTypes.TypeList)
            {
                AddGlobalCommonComponentsAttribute addGlobalCommonComponentsAttribute = type.GetAttribute<AddGlobalCommonComponentsAttribute>();

                if (addGlobalCommonComponentsAttribute == null)
                {
                    continue;
                }  

                foreach (var globalSettingsType in addGlobalCommonComponentsAttribute.Types)
                {
                    CreateIfMissingType(globalSettingsType);
                }
            }
        }
        
        public override void OnRemoveInvalidElements()
        {
            for (int i = ElementList.Count - 1; i >= 0; i--)
            {
                bool find = false;
                
                foreach (var type in AllToolTypes.TypeList)
                {
                    AddGlobalCommonComponentsAttribute attribute = 
                        (AddGlobalCommonComponentsAttribute)type.GetAttribute(typeof(AddGlobalCommonComponentsAttribute));

                    if (attribute == null)
                    {
                        continue;
                    }

                    if (attribute.Types.Contains(ElementList[i].GetType()))
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    Remove(i);
                }
            }
        }
    }
}