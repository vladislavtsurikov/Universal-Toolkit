using UnityEditor;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.PhysicsSimulator.Runtime.Settings
{
    [LocationAsset("Physics Simulation Settings")]
    public class PhysicsSimulatorSettings : SerializedScriptableObjectSingleton<PhysicsSimulatorSettings>
    {
        public AutoPositionDownSettings AutoPositionDownSettings = new AutoPositionDownSettings();
        
        public bool SimulatePhysics = true;
        public float GlobalDisablePhysicsTime = 20f;
        public float DisablePhysicsTime = 6f;
        public int SpeedUpPhysics = 4;

#if UNITY_EDITOR
        public void Save() 
        {
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
