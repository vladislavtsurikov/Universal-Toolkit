#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using VladislavTsurikov.Coroutines.Runtime;

namespace VladislavTsurikov.PhysicsSimulatorEditor.Editor
{
    public static class ObjectTimePhysicsSimulator
    {
        private static bool _pastSimulatePhysics;

        public static void SimulatePhysics() 
        {
            DisablePhysicsSupportWithDelay();

            PhysicsSimulator.Simulate();
        }

        private static void DisablePhysicsSupportWithDelay() 
        {            
            if(SimulatedBodyStack.SimulatedBodyList.Count == 0)
            {
                return;
            }

            if(PhysicsSimulatorSettings.Instance.SimulatePhysics)
            {
                if(!_pastSimulatePhysics)
                {
                    foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyList)
                    {
                        CoroutineRunner.StartCoroutine(DisablePhysicsSupportWithDelay(PhysicsSimulatorSettings.Instance.ObjectTime, simulatedBody), simulatedBody.GameObject);
                    }
                }

                _pastSimulatePhysics = true;
            }
            else
            {
                if(_pastSimulatePhysics)
                {
                    foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyList)
                    {
                        CoroutineRunner.StopCoroutine(DisablePhysicsSupportWithDelay(PhysicsSimulatorSettings.Instance.ObjectTime, simulatedBody), simulatedBody.GameObject);               
                    }        
                }

                _pastSimulatePhysics = false;      
            }
        }

        public static void StopAllCoroutine() 
        {            
            foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyList)
            {
                CoroutineRunner.StopCoroutine(DisablePhysicsSupportWithDelay(PhysicsSimulatorSettings.Instance.ObjectTime, simulatedBody), simulatedBody.GameObject);               
            } 
        }

        public static IEnumerator DisablePhysicsSupportWithDelay(float waitForSeconds, SimulatedBody simulatedBody) 
        {
            yield return new WaitForSeconds(waitForSeconds);
            
            SimulatedBodyStack.DisablePhysicsSupport(simulatedBody);
        }
    }
}
#endif