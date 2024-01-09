using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    [MenuItem("Stamper Tool Controller")]
    public class StamperControllerSettings : Component
    {
        public bool Visualisation = true;
        public bool AutoRespawn;
        public float DelayAutoRespawn;
        
        [NonSerialized]
        public StamperTool StamperTool;

        protected override void SetupElement(object[] args = null)
        {
            StamperTool = (StamperTool)args[0];
        }
    }
}