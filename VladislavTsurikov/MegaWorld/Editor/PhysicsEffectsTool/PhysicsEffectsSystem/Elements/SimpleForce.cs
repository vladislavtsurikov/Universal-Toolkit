#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem
{
    [ComponentStack.Runtime.AdvancedComponentStack.Name("Simple Force")]
	public class SimpleForce : PhysicsEffect
    {
        public float MaxForce => 500;

        public float Angle = 0;
        public float Force = 1f;
            
        public override void ApplyEffect(Vector3 positionOffsetY, Rigidbody rb)
        {
            float radians = Angle * Mathf.Deg2Rad;

            Vector3 forceDirection = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));

            forceDirection *= Force;

            rb.AddForce(forceDirection);
        }

        public override void OnRepaint(RayHit rayHit)
        {
            Vector3 positionUp = GetPositionOffsetY(rayHit);

            Handles.color = new Vector4(0, 1, 0, GetOpacity(Force, MaxForce));
            Handles.SphereHandleCap(0, positionUp, Quaternion.identity, Size, EventType.Repaint);
            Handles.color = Color.green;
            Handles.DrawDottedLine(rayHit.Point, positionUp, 2f);

            Handles.SphereHandleCap(0, positionUp, Quaternion.identity, 0.3f, EventType.Repaint);

            float radians = Angle * Mathf.Deg2Rad;

            Vector3 forceDirection = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));

            var normVect = Vector3.Normalize(forceDirection);
            Handles.DrawLine(positionUp, positionUp + normVect * (Size / 2));
            Handles.SphereHandleCap(0, positionUp + normVect * (Size / 2), Quaternion.identity, 0.3f, EventType.Repaint);
        }
	}
}
#endif
