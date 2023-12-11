using System;
using UnityEngine;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents
{
    [Serializable]
    public abstract class ModifyTransformComponent : ComponentStack.Runtime.Component
    {
        public virtual void SetInstanceData(ref InstanceData spawnInfo, ref ModifyInfo modifyInfo, float moveLenght, Vector3 strokeDirection, float fitness, Vector3 normal) {}
    }
}
