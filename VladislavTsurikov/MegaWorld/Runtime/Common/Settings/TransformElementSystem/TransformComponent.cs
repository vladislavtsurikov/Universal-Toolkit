using UnityEngine;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    public abstract class TransformComponent : ComponentStack.Runtime.Component
    {
        public virtual void SetInstanceData(ref InstanceData instanceData, float fitness, Vector3 normal) {}
    }
}
