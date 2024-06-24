using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings
{
    [Serializable]
    [MenuItem("Precise Place Settings")]
    public class PrecisePlaceSettings : Component
    {
        public float PositionOffset;
    }
}