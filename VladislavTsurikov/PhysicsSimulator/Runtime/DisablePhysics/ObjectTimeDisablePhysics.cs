using System.Collections;
using UnityEngine;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.PhysicsSimulator.Runtime.Settings;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.Utility.Runtime.Extensions;
using Coroutine = VladislavTsurikov.Coroutines.Runtime.Coroutine;

namespace VladislavTsurikov.PhysicsSimulator.Runtime.DisablePhysics
{
    public class ObjectTimeDisablePhysics : DisablePhysics
    {
        private static bool _pastSimulatePhysics;

        internal override void DisablePhysicsSupport()
        {
            if(SimulatedBodyStack.SimulatedBodyHashSet.Count == 0)
            {
                return;
            }

            if(PhysicsSimulatorSettings.Instance.SimulatePhysics)
            {
                if(!_pastSimulatePhysics)
                {
                    foreach (SimulatedBody.SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyHashSet)
                    {
                        CoroutineRunner.StartCoroutine(DisablePhysicsSupport(PhysicsSimulatorSettings.Instance.DisablePhysicsTime, simulatedBody), simulatedBody.GameObject);
                    }
                }

                _pastSimulatePhysics = true;
            }
            else
            {
                if(_pastSimulatePhysics)
                {
                    foreach (SimulatedBody.SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyHashSet)
                    {
                        CoroutineRunner.StopCoroutine(DisablePhysicsSupport(PhysicsSimulatorSettings.Instance.DisablePhysicsTime, simulatedBody), simulatedBody.GameObject);               
                    }        
                }

                _pastSimulatePhysics = false;      
            }
        }
        
        internal override void OnDisable()
        {
            foreach (SimulatedBody.SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyHashSet)
            {
                CoroutineRunner.StopCoroutine(DisablePhysicsSupport(PhysicsSimulatorSettings.Instance.DisablePhysicsTime, simulatedBody), simulatedBody.GameObject);               
            }
        }
        
        internal override void OnRegisterSimulatedBody(SimulatedBody.SimulatedBody simulatedBody)
        {
            if (PhysicsSimulatorSettings.Instance.SimulatePhysics)
            {
                CoroutineRunner.StartCoroutine(DisablePhysicsSupport(PhysicsSimulatorSettings.Instance.DisablePhysicsTime, simulatedBody), simulatedBody.GameObject);
            }
        }
        
        internal override void OnUnregisterSimulatedBody(SimulatedBody.SimulatedBody simulatedBody)
        {
            CoroutineRunner.StopCoroutine(DisablePhysicsSupport(PhysicsSimulatorSettings.Instance.DisablePhysicsTime, simulatedBody), simulatedBody.GameObject);
        }

        private static IEnumerator DisablePhysicsSupport(float waitForSeconds, SimulatedBody.SimulatedBody simulatedBody) 
        {
            yield return new WaitForSeconds(waitForSeconds);
            
            SimulatedBodyStack.DisablePhysicsSupport(simulatedBody);
        }
    }
}