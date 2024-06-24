using System;
using UnityEngine;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.PhysicsSimulator.Runtime.DisablePhysics;
using VladislavTsurikov.PhysicsSimulator.Runtime.Settings;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.PhysicsSimulator.Runtime
{
    public static class PhysicsSimulator
    {
        private static bool _useAccelerationPhysics = true;

        public static DisablePhysics.DisablePhysics ActiveDisablePhysics { get; private set; } = new ObjectTimeDisablePhysics();

        static PhysicsSimulator() 
        {
            EditorAndRuntimeUpdate.AddUpdateFunction(SimulatePhysicsIfNecessary);
        }

        private static void SimulatePhysicsIfNecessary() 
        {
            if (SimulatedBodyStack.SimulatedBodyHashSet.Count == 0)
            {
                return;
            }

            ActiveDisablePhysics.DisablePhysicsSupport();

            if (PhysicsSimulatorSettings.Instance.SimulatePhysics)
            {
                SimulatePhysics();
            }
        }

        private static void SimulatePhysics()
        {
#if UNITY_2022_3_OR_NEWER
            SimulationMode simulationMode = Physics.simulationMode;

            Physics.simulationMode = SimulationMode.Script;
#else
            bool prevAutoSimulation = Physics.autoSimulation;

            Physics.autoSimulation = false;
#endif

            float accelerationPhysics = _useAccelerationPhysics ? PhysicsSimulatorSettings.Instance.SpeedUpPhysics : 1;
            
            for (int i = 0; i < accelerationPhysics; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);

                SimulatedBodyStack.DisablePhysicsSupportIfObjectStopped();
                
                if(SimulatedBodyStack.SimulatedBodyHashSet.Count == 0)
                {
                    break;
                }
            }
            
#if UNITY_2022_3_OR_NEWER
            Physics.simulationMode = simulationMode;
#else
            Physics.autoSimulation = prevAutoSimulation;
#endif
        }

        public static void Activate<T>(bool useAccelerationPhysics = true) where T : DisablePhysics.DisablePhysics
        {
            if(ActiveDisablePhysics.GetType() != typeof(T))
            {
                ActiveDisablePhysics.OnDisable();

                ActiveDisablePhysics = Activator.CreateInstance<T>();
            }

            _useAccelerationPhysics = useAccelerationPhysics;
            ActiveDisablePhysics.OnActivate();
        }
    }
}