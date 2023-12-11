#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VladislavTsurikov.PhysicsSimulatorEditor.Editor
{
    public static class SimulatedBodyStack 
    {
        internal static readonly List<SimulatedBody> SimulatedBodyList = new List<SimulatedBody>();
        
        public static void DisablePhysicsSupportIfObjectStopped() 
        {            
            if(!PhysicsSimulatorSettings.Instance.SimulatePhysics)
            {
                return;
            }

            List<SimulatedBody> removeSimulatedBodyList = new List<SimulatedBody>();

            foreach (SimulatedBody simulatedBody in SimulatedBodyList)
            {
                if(simulatedBody.IsRigidbodyStopping())
                {
                    removeSimulatedBodyList.Add(simulatedBody);
                }
            }

            foreach (SimulatedBody simulatedBody in removeSimulatedBodyList)
            {
                DisablePhysicsSupport(simulatedBody);
            }
        }

        public static SimulatedBody GetSimulatedBody(GameObject gameObject)
        {
            foreach (SimulatedBody simulatedBody in SimulatedBodyList)
            {
                if(simulatedBody.GameObject == null)
                {
                    continue;
                }

                if(simulatedBody.GameObject == gameObject)
                {
                    return simulatedBody;
                }
            }

            return null;
        }

        public static void DisableAllPhysicsSupport() 
        {
            List<SimulatedBody> removeSimulatedBodyList = new List<SimulatedBody>();
            removeSimulatedBodyList.AddRange(SimulatedBodyList);

            foreach (SimulatedBody simulatedBody in removeSimulatedBodyList)
            {
                DisablePhysicsSupport(simulatedBody);
            }
        }

        public static void DisablePhysicsSupport(SimulatedBody simulatedBody) 
        {
            if(simulatedBody.GameObject == null)
            {
                SimulatedBodyList.Remove(simulatedBody);
                return;
            }

            if(!simulatedBody.HasRigidbody())
            {
                return;
            }

            simulatedBody.DisablePhysicsSupport();

            SimulatedBodyList.Remove(simulatedBody);
        }
    }
}
#endif