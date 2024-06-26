using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings
{
    [Name("Physics Effects")]
    public class PhysicsEffects : Component
    {
        #region Force
        public bool ForceRange = true;
        public float MinForce = 10f;
        public float MaxForce = 40f;
        #endregion

        #region Direction
        public float RandomStrength = 50f;
        #endregion

        public void ApplyForce(Rigidbody rigidbody) 
        {
            if (rigidbody == null)
            {
                return;
            }

            float radians = Random.Range(0, 360) * Mathf.Deg2Rad;

            Vector3 forceDirection = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));

            Vector3 force = Vector3.Lerp(new Vector3(0, -1, 0), forceDirection, RandomStrength / 100);
            
            float magnitude = ForceRange ? Random.Range(MinForce, MaxForce) : MinForce;

            force *= magnitude;

            PhysicsUtility.ApplyForce(rigidbody, force);
        }
    }
}