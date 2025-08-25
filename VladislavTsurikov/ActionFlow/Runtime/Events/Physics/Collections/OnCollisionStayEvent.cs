using VladislavTsurikov.ReflectionUtility;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.Physics
{
    [Name("Physics/On Collision Stay")]
    public class OnCollisionStayEvent : PhysicsEvent
    {
        protected internal override void OnCollisionStay(Collision collision)
        {
            Trigger.Run();
        }
    }
}