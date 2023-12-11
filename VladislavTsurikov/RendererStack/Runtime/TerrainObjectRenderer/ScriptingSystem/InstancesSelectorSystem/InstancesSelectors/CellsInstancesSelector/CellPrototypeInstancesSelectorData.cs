using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem.InstancesSelectors.CellsInstancesSelector
{
    public class CellPrototypeInstancesSelectorData : PrototypeInstancesSelectorData
    {
        public List<ColliderCell> LastColliderCellList = new List<ColliderCell>();

        private Vector3 _lastCenter;
        private float _lastDistance;

        public CellPrototypeInstancesSelectorData(object obj) : base(obj)
        {
            
        }

        public override bool IsNeedUpdate(Sphere sphere)
        {
            bool needsUpdate = false;
            
            float distance = Vector3.Distance(_lastCenter, sphere.Center);
            if (distance > TerrainObjectRendererCollider.ConstCellSize || Mathf.Abs(_lastDistance - sphere.Radius) > 0.1f)
            {
                needsUpdate = true;
                _lastCenter = sphere.Center;
                _lastDistance = sphere.Radius;
            }

            return needsUpdate;
        }
    }
}