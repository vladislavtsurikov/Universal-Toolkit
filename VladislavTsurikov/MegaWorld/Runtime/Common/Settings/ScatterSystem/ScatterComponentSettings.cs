using System;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Serializable]
    [Name("Scatter Settings")]
    public class ScatterComponentSettings : Component
    {
        public ScatterStack ScatterStack = new ScatterStack();

        protected override void SetupComponent(object[] setupData = null)
        {
            ScatterStack.Setup();
        }

        protected override void OnCreate()
        {
            ScatterStack.CreateIfMissingType(typeof(RandomGrid));
        }
    }
}