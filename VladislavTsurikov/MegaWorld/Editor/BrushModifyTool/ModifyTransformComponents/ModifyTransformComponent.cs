using System;
using UnityEngine;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Serializable]
    public abstract class ModifyTransformComponent : ComponentStack.Runtime.Component
    {
        public virtual void SetInstanceData(ref Transform spawnInfo, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal) {}
    }
}
