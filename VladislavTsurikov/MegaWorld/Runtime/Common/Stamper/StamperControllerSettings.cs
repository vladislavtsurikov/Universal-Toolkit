using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    [MenuItem("Stamper Tool Controller")]
    public class StamperControllerSettings : Component
    {
        public bool Visualisation = true;
        public bool AutoRespawn;
        public float DelayAutoRespawn = 0.1f;
        
        [NonSerialized]
        public StamperTool StamperTool;

        protected override void SetupElement(object[] args = null)
        {
            StamperTool = (StamperTool)args[0];
        }
    }
}