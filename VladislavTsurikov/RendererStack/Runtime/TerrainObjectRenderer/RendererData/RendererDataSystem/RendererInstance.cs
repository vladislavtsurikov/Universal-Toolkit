using System;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem
{
    [Serializable]
    public struct RendererInstance
    {
        public int ID;
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;

        public static RendererInstance ConvertToSerializableInstance(TerrainObjectInstance instance)
        {
            RendererInstance item;
            item.ID = instance.ID;
            item.Position = instance.Position;
            item.Rotation = instance.Rotation;
            item.Scale = instance.Scale;

            return item;
        }
    }
}