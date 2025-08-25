using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.Physics
{
    [Name("Physics/On Trigger Enter")]
    public class OnTriggerEnterEvent : PhysicsEvent
    {
        protected internal override void OnTriggerEnter(Collider other)
        {
            Trigger.Run();
        }
    }
}