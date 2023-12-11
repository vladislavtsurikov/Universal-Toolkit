using UnityEngine;

namespace VladislavTsurikov.Math.Runtime
{
    public struct Sphere
    {
        private bool _isValid;

        public bool IsValid => _isValid;
        
        public Vector3 Center;
        public float Radius;

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
            _isValid = true;
        }
        
        public bool Contains(Vector3 point)
        {
            return Vector3.Distance(point, Center) <= Radius;
        }
        
        public bool Intersects(Sphere otherSphere)
        {
            float distance = Vector3.Distance(Center, otherSphere.Center);

            if (distance < Radius + otherSphere.Radius)
            {
                return true;
            }

            return false;
        }
        
        public bool Intersects(OBB obb)
        {
            return obb.IntersectsSphere(Center, Radius);
        }
        
        public bool Intersects(AABB aabb)
        {
            return aabb.IntersectsSphere(Center, Radius);
        }
    }
}