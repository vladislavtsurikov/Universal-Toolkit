using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    public enum RaycastType
    {
        UnityRaycast,
        CustomRaycast
    }
    
    [MenuItem("Raycast")]
    public class RaycastPreferenceSettings : PreferenceSettings
    {
        public RaycastType RaycastType = RaycastType.UnityRaycast;
        public float MaxRayDistance = 6500f;
        public float Offset = 500;
    }
}
