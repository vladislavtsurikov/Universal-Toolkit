using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    [Serializable]
    [MenuItem("Spawn Detail Settings")]
    public class SpawnDetailSettings : Component
    {
        public bool UseRandomOpacity = true;
        public int Density = 5;
        public float FailureRate;
    }
}

