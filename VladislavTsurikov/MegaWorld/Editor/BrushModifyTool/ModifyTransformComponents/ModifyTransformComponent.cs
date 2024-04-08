using System;
using UnityEngine;
using Transform = VladislavTsurikov.Core.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Serializable]
    public abstract class ModifyTransformComponent : ComponentStack.Runtime.Component
    {
        public virtual void ModifyTransform(ref Transform spawnInfo, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal) {}
    }
}
