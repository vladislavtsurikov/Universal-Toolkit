using UnityEngine;
using VladislavTsurikov.PhysicsSimulator.Runtime.Settings;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;

namespace VladislavTsurikov.PhysicsSimulator.Runtime.DisablePhysics
{
    public class GlobalTimeDisablePhysics : DisablePhysics
    {
        private float _timeToFinish;

        internal override void DisablePhysicsSupport()
        {
            if (_timeToFinish <= Time.time)
            {
                _timeToFinish = Time.time + PhysicsSimulatorSettings.Instance.GlobalDisablePhysicsTime;
                SimulatedBodyStack.DisableAllPhysicsSupport();
            }
        }

        internal override void OnActivate()
        {
            _timeToFinish = Time.time + PhysicsSimulatorSettings.Instance.GlobalDisablePhysicsTime;
        }
    }
}