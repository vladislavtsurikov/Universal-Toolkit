using System;
using UnityEngine;
using VladislavTsurikov.PhysicsSimulator.Runtime.SimulatedBody;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.PhysicsSimulator.Runtime.Settings
{
    [Serializable]
    public class AutoPositionDownSettings
    {
        public bool EnableAutoPositionDown = true;
        public float PositionDown = 50;
        
        public void ApplyOffset(GameObject go) 
        {
            if(!EnableAutoPositionDown)
            {
                return;
            }
            
            Bounds bounds = go.GetInstantiatedBounds();

            var position = go.transform.position;
            position = new Vector3(position.x, position.y - Mathf.Lerp(0, bounds.extents.y, PositionDown / 100), position.z);
            go.transform.position = position;
        }
    }

    public class ApplyPositionDown : OnDisableSimulatedBodyAction
    {
        private AutoPositionDownSettings _autoPositionDownSettings;
        
        public ApplyPositionDown(AutoPositionDownSettings autoPositionDownSettings)
        {
            _autoPositionDownSettings = autoPositionDownSettings;
        }

        protected override void OnDisablePhysics()
        {
            _autoPositionDownSettings.ApplyOffset(SimulatedBody.GameObject);
        }
    }
}