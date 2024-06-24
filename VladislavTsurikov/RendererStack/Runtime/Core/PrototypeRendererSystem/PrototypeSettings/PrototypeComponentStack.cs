using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    public class PrototypeComponentStack : ComponentStackOnlyDifferentTypes<PrototypeComponent>
    {
        public PrototypeComponentStack(Type rendererType, Prototype prototype)
        {
            InitializationDataForElements = new object[]{prototype};
            
            foreach (var type in rendererType.GetAttribute<AddPrototypeComponentsAttribute>().PrototypeSettings)
            {
                CreateIfMissingType(type);
            }
        }
    }
}