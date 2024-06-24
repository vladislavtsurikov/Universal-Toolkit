using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Serializable]
    [MenuItem("Scatter Settings")]
    public class ScatterComponentSettings : Component
    {
        public ScatterStack ScatterStack = new ScatterStack();

        protected override void SetupElement(object[] args = null)
        {
            ScatterStack.Setup();
        }

        protected override void OnCreate()
        {
            ScatterStack.CreateIfMissingType(typeof(RandomGrid));
        }
    }
}