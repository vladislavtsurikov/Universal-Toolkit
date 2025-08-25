using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.Physics
{
    [Name("Physics/On Trigger Exit")]
    public class OnTriggerExitEvent : PhysicsEvent
    {
        protected internal override void OnTriggerExit(Collider other)
        {
            Trigger.Run();
        }
    }
}