using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem.Components;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Serializable]
    [MenuItem("Scatter Settings")]
    public class ScatterComponentSettings : Component
    {
        public ScatterStack Stack = new ScatterStack();

        protected override void SetupElement(object[] args = null)
        {
            Stack.Setup();
        }

        protected override void OnCreate()
        {
            Stack.CreateIfMissingType(typeof(RandomGrid));
        }
    }
}