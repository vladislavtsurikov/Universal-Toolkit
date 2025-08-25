using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.Physics
{
    [Name("Physics/On Collision Enter")]
    public class OnCollisionEnterEvent : PhysicsEvent
    {
        protected internal override void OnCollisionEnter(Collision collision)
        {
            Trigger.Run();
        }
    }
}