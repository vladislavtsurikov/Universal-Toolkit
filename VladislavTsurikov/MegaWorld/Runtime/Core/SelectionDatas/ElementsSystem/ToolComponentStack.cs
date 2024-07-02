using System;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    public class ToolComponentStack : Component
    {
        public Type ToolType;

        public ComponentStackOnlyDifferentTypes<Component> ComponentStack = 
            new ComponentStackOnlyDifferentTypes<Component>();

        protected override void SetupComponent(object[] setupData = null)
        {
            ComponentStack.Setup();
        }

        public override bool IsValid()
        {
            return ToolType != null;
        }
    }
}