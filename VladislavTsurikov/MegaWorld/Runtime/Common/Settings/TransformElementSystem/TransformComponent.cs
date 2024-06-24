using UnityEngine;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    public abstract class TransformComponent : Component
    {
        public virtual void SetInstanceData(ref Instance instance, float fitness, Vector3 normal) {}
    }
}
