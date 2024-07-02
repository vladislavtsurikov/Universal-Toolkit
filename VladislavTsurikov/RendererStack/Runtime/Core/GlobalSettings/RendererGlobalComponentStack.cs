using System;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings
{
    public class RendererGlobalComponentStack : Component
    {
        public Type RendererType;
        
        [OdinSerialize]
        public ComponentStackOnlyDifferentTypes<GlobalComponent> ComponentStack = 
            new ComponentStackOnlyDifferentTypes<GlobalComponent>();

        protected override void SetupComponent(object[] setupData = null)
        {
            ComponentStack.Setup();
        }

        public override bool IsValid()
        {
            return RendererType != null;
        }
    }
}