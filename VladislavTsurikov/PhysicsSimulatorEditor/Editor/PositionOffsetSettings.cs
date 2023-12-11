#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.PhysicsSimulatorEditor.Editor
{
    [Serializable]
    public class PositionOffsetSettings
    {
        public bool EnableAutoOffset = true;
        public float PositionOffsetDown = 50;
        
        public void ApplyOffset(GameObject go) 
        {
            if(!EnableAutoOffset)
            {
                return;
            }
            
            Bounds bounds = go.GetInstantiatedBounds();

            var position = go.transform.position;
            position = new Vector3(position.x, position.y - Mathf.Lerp(0, bounds.extents.y, PositionOffsetDown / 100), position.z);
            go.transform.position = position;
        }
    }
}
#endif
