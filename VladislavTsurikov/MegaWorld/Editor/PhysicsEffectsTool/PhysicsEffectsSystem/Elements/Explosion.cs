#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem.Elements
{
    [ComponentStack.Runtime.Attributes.MenuItem("Explosion")]
	public class Explosion : PhysicsEffect
    {
        public float MaxForce => 500;
        
        public float Force = 50f;

        public override void ApplyEffect(Vector3 positionOffsetY, Rigidbody rb)
        {            
            rb.AddExplosionForce(Force, positionOffsetY, Size / 2, 4.0F);
        }

        public override void OnRepaint(RayHit rayHit)
        {
            Vector3 positionUp = GetPositionOffsetY(rayHit);

            Handles.color = new Vector4(1, 0, 0, GetOpacity(Force, MaxForce));

            Handles.SphereHandleCap(0, positionUp, Quaternion.identity, Size, EventType.Repaint);
            Handles.color = Color.red;
            Handles.DrawDottedLine(rayHit.Point, positionUp, 2f);
            Handles.SphereHandleCap(0, positionUp, Quaternion.identity, 0.3f, EventType.Repaint);
        }
    }
}
#endif