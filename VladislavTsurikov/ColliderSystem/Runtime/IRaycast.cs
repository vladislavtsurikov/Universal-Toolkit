using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public interface IRaycast
    {
        public List<RayHit> RaycastAll(Ray ray, ObjectFilter objectFilter);
    }
}