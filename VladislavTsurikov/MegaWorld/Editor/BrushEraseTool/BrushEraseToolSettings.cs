#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool
{
    [Serializable]
    [MenuItem("Brush Erase Tool Settings")]
    public class BrushEraseToolSettings : Component
    {   
        public float EraseStrength = 1.0f;
    }   
}
#endif