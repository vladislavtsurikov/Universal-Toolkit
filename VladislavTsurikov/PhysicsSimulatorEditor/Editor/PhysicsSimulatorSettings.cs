#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.PhysicsSimulatorEditor.Editor
{
    [LocationAsset("Physics Simulation Settings")]
    public class PhysicsSimulatorSettings : SerializedScriptableObjectSingleton<PhysicsSimulatorSettings>
    {
        public PositionOffsetSettings PositionOffsetSettings = new PositionOffsetSettings();
        
        public bool SimulatePhysics = true;
        public float GlobalTime = 20f;
        public float ObjectTime = 6f;
        public int AccelerationPhysics = 4;

        public void Save() 
        {
            EditorUtility.SetDirty(this);
        }
    }
}
#endif