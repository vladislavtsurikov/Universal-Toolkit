using UnityEngine;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    public abstract class TransformComponent : ComponentStack.Runtime.Component
    {
        public virtual void SetInstanceData(ref Transform transform, float fitness, Vector3 normal) {}
    }
}
