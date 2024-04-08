using UnityEngine;

namespace VladislavTsurikov.Core.Runtime
{
    public class Transform
    {
        public Vector3 Position;
        public Vector3 Scale; 
        public Quaternion Rotation;

        public Transform(GameObject gameObject)
        {
            Position = gameObject.transform.position;
            Scale = gameObject.transform.localScale; 
            Rotation = gameObject.transform.rotation;
        }
        
        public Transform(Vector3 position, Vector3 scale, Quaternion rotation)
        {
            Position = position;
            Scale = scale; 
            Rotation = rotation;
        }
    }
}

