using System;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    [Name("Stamper Tool Controller")]
    public class StamperControllerSettings : Component
    {
        public bool Visualisation = true;
        public bool AutoRespawn;
        public float DelayAutoRespawn = 0.1f;
        
        [NonSerialized]
        public StamperTool StamperTool;

        protected override UniTask SetupComponent(object[] setupData = null)
        {
            StamperTool = (StamperTool)setupData[0];
            return UniTask.CompletedTask;
        }
    }
}