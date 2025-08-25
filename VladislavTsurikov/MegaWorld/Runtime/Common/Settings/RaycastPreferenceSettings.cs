﻿using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings
{
    public enum RaycastType
    {
        UnityRaycast,
        CustomRaycast
    }
    
    [Name("Raycast")]
    public class RaycastPreferenceSettings : PreferenceSettings
    {
        public RaycastType RaycastType = RaycastType.UnityRaycast;
        public float MaxRayDistance = 6500f;
        public float Offset = 500;
    }
}
