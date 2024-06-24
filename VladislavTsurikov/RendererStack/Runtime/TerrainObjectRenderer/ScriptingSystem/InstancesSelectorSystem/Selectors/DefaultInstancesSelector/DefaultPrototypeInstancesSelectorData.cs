using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem.DefaultInstancesSelector
{
    public class DefaultPrototypeInstancesSelectorData : PrototypeInstancesSelectorData
    {
        public Sphere LastDistanceCullingSphere;
        public List<ColliderObject> LastInstanceList = new List<ColliderObject>();
        
        private Vector3 _lastCenter;
        private float _lastDistance;
        private float _lastDistanceFromCenter;

        public DefaultPrototypeInstancesSelectorData(object obj) : base(obj) {}
        
        public override bool IsNeedUpdate(Sphere sphere)
        {
            bool needsUpdate = false;
            
            float distance = Vector3.Distance(_lastCenter, sphere.Center);
            if (Mathf.Abs(_lastDistanceFromCenter - distance) > 1f || Mathf.Abs(_lastDistance - sphere.Radius) > 0.1f)
            {
                needsUpdate = true;
                _lastDistanceFromCenter = distance;
                _lastCenter = sphere.Center;
                _lastDistance = sphere.Radius;
            }

            return needsUpdate;
        }
    }
}