using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    public class TransformComponentStack : ComponentStackOnlyDifferentTypes<TransformComponent>
    {
        public void ManipulateTransform(ref Transform transform, float fitness, Vector3 normal)
        {
            foreach (TransformComponent item in _elementList)
            {
                if(item.Active)
                {
                    item.SetInstanceData(ref transform, fitness, normal);
                }
            }
        }
    }
}
