using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings.OverlapChecks
{
    [Serializable]
    public class CollisionCheck
    {
        public float MultiplyBoundsSize = 1;
        public bool CollisionCheckType;
        public LayerMask CheckCollisionLayers;

        public bool IsBoundHittingWithCollisionsLayers(Vector3 position, float rotation, Vector3 extents)
        {
            if (Physics.BoxCast(new Vector3(position.x, position.y + PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.Offset, position.z), extents, 
                    Vector3.down, out _, Quaternion.Euler(0f, rotation, 0f), PreferenceElementSingleton<RaycastPreferenceSettings>.Instance.Offset + 1000f, CheckCollisionLayers))
            {
                return true;
            }

            return false;
        }
        
        public bool RunCollisionCheck(Vector3 prefabExtents, Transform transform)
        {
            if(CollisionCheckType)
            {
                Vector3 extents = Vector3.Scale(prefabExtents * MultiplyBoundsSize, transform.Scale);

                if(IsBoundHittingWithCollisionsLayers(transform.Position, transform.Rotation.eulerAngles.y, extents))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
