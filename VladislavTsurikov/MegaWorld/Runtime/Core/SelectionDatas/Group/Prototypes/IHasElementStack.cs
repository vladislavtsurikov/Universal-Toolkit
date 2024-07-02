using System;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes
{
    public interface IHasElementStack
    {
        ComponentStackManager ComponentStackManager { get;}

        public void SetupComponentStack();

        public Component GetElement(Type elementType)
        {
            return ComponentStackManager.GeneralComponentStack.GetElement(elementType);
        }

        public Component GetElement(Type toolType, Type elementType)
        {
            return ComponentStackManager.ToolsComponentStack.GetElement(elementType, toolType);
        }
    }
}