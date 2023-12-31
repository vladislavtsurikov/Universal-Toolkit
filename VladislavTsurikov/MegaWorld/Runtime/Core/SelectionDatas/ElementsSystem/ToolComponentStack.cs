﻿using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    public class ToolComponentStack : Component
    {
        public Type ToolType;

        public ComponentStackOnlyDifferentTypes<Component> ComponentStack = 
            new ComponentStackOnlyDifferentTypes<Component>();

        protected override void SetupElement(object[] args = null)
        {
            ComponentStack.Setup();
        }

        public override bool IsValid()
        {
            return ToolType != null;
        }
    }
}