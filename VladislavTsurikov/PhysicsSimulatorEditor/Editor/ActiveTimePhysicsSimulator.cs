#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulatorEditor.Editor
{
    public static class ActiveTimePhysicsSimulator
    {
        private static float _sActiveTime;

        public static void SimulatePhysics() 
        {
            _sActiveTime += Time.deltaTime;

            if (_sActiveTime >= PhysicsSimulatorSettings.Instance.GlobalTime)
            {
                _sActiveTime = 0f;
                SimulatedBodyStack.DisableAllPhysicsSupport();
            }
            else
            {
                PhysicsSimulator.Simulate();
            }
        }

        public static void RefreshTime()
        {
            _sActiveTime = 0f;
        }
    }
}
#endif
