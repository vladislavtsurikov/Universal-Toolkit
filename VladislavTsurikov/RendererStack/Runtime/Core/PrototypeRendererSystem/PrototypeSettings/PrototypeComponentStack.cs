using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    public class PrototypeComponentStack : ComponentStackOnlyDifferentTypes<PrototypeComponent>
    {
        internal void CreateAllComponents()
        {
            Type rendererType = (Type)SetupData[0];
            
            foreach (var type in rendererType.GetAttribute<AddPrototypeComponentsAttribute>().PrototypeSettings)
            {
                CreateIfMissingType(type);
            }
        }
        
        protected override void OnCreateElements()
        {
            Type rendererType = (Type)SetupData[0];
            
            foreach (var type in rendererType.GetAttribute<AddPrototypeComponentsAttribute>().PrototypeSettings)
            {
                if (type.GetAttribute<PersistentComponentAttribute>() != null)
                {
                    CreateIfMissingType(type);
                }
            }
        }
    }
}