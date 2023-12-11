using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings
{
    [Serializable]
    [MenuItem("Precise Place Settings")]
    public class PrecisePlaceSettings : Component
    {
        public float PositionOffset;
    }
}