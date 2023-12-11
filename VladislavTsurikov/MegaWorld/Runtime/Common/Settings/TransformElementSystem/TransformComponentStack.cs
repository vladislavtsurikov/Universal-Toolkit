using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    public class TransformComponentStack : ComponentStackOnlyDifferentTypes<TransformComponent>
    {
        public void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal)
        {
            foreach (TransformComponent item in _elementList)
            {
                if(item.Active)
                {
                    item.SetInstanceData(ref instanceData, fitness, normal);
                }
            }
        }
    }
}
