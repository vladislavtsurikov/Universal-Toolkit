#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.GameObjectCollider.Runtime.Utility;

namespace VladislavTsurikov.PhysicsSimulatorEditor.Editor
{
    public enum DisablePhysicsMode
    {
        GlobalTime,
        ObjectTime
    }

    [InitializeOnLoad]
    public static class PhysicsSimulator
    {
        private static DisablePhysicsMode _sDisablePhysicsTimeMode = DisablePhysicsMode.GlobalTime;
        private static bool _sUseAccelerationPhysics = true;
        private static bool _sActive;
        private static bool _sEnablePermanentPhysics;

        static PhysicsSimulator() 
        {
            EditorApplication.update -= SimulatePhysics;
            EditorApplication.update += SimulatePhysics;
        }

        public static void Simulate()
        {
            bool prevAutoSimulation = Physics.autoSimulation;
            
            Physics.autoSimulation = false;

            float accelerationPhysics = _sUseAccelerationPhysics ? PhysicsSimulatorSettings.Instance.AccelerationPhysics : 1;
            
            for (int i = 0; i < accelerationPhysics; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);

                SimulatedBodyStack.DisablePhysicsSupportIfObjectStopped();
                if(SimulatedBodyStack.SimulatedBodyList.Count == 0)
                {
                    break;
                }
            }

            Physics.autoSimulation = prevAutoSimulation;
        }
        
        public static void Activate(DisablePhysicsMode disablePhysicsTimeMode, bool useAccelerationPhysics = true, bool enablePermanentPhysics = false)
        {
            if(_sDisablePhysicsTimeMode != disablePhysicsTimeMode)
            {
                if(_sDisablePhysicsTimeMode == DisablePhysicsMode.ObjectTime)
                {
                    ObjectTimePhysicsSimulator.StopAllCoroutine();
                }
            }

            _sDisablePhysicsTimeMode = disablePhysicsTimeMode;
            _sUseAccelerationPhysics = useAccelerationPhysics;

            _sActive = true;
            _sEnablePermanentPhysics = enablePermanentPhysics;
        }

        private static void SimulatePhysics() 
        {
            if (!_sActive)
            {
                if(!_sEnablePermanentPhysics)
                {
                    return;
                }
            }

            if (PhysicsSimulatorSettings.Instance.SimulatePhysics) 
            {
                switch (_sDisablePhysicsTimeMode)
                {
                    case DisablePhysicsMode.GlobalTime:
                    {
                        ActiveTimePhysicsSimulator.SimulatePhysics();
                        break;
                    }
                    case DisablePhysicsMode.ObjectTime:
                    {
                        ObjectTimePhysicsSimulator.SimulatePhysics();
                        break;
                    }
                }
            }

            if(SimulatedBodyStack.SimulatedBodyList.Count == 0)
            {
                _sActive = false;
            }
        }

        public static void RegisterGameObject(SimulatedBody simulatedBody)
        {
            if(SimulatedBodyStack.GetSimulatedBody(simulatedBody.GameObject) == null)
            {
                simulatedBody.OnAddToSimulatedBodyStack?.Invoke();
                SimulatedBodyStack.SimulatedBodyList.Add(simulatedBody);
            }

            if(_sDisablePhysicsTimeMode == DisablePhysicsMode.ObjectTime)
            {
                if(PhysicsSimulatorSettings.Instance.SimulatePhysics)
                {
                    CoroutineRunner.StartCoroutine(ObjectTimePhysicsSimulator.DisablePhysicsSupportWithDelay(PhysicsSimulatorSettings.Instance.ObjectTime, simulatedBody), simulatedBody.GameObject);
                }
            }
        }
    }
}
#endif