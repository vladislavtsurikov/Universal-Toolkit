using System;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    public class ToolComponentStack : Component
    {
        public ComponentStackOnlyDifferentTypes<Component> ComponentStack = new();

        public Type ToolType;

        protected override async UniTask SetupComponent(object[] setupData = null) => await ComponentStack.Setup();

        public override bool DeleteElement() => ToolType != null;
    }
}
