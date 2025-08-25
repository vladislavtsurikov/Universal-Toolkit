using System;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem
{
    [Serializable]
    [Name("Scatter Settings")]
    public class ScatterComponentSettings : Component
    {
        public ScatterStack ScatterStack = new ScatterStack();

        protected override async UniTask SetupComponent(object[] setupData = null)
        {
            await ScatterStack.Setup();
        }

        protected override void OnCreate()
        {
            ScatterStack.CreateIfMissingType(typeof(RandomGrid));
        }
    }
}