using System.Collections.Generic;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData
{
    public static class ModifiedPrototypeRenderDataStack
    {
        private static List<PrototypeRendererData> _prototypeRenderData = new List<PrototypeRendererData>();

        public static void Add(PrototypeRendererData prototypeRendererData)
        {
            if(!_prototypeRenderData.Contains(prototypeRendererData))
            {
                _prototypeRenderData.Add(prototypeRendererData);
            }
        }

        public static void ConvertPersistentDataToTemporaryData()
        {
            foreach (PrototypeRendererData item in _prototypeRenderData)
            {
                item?.ConvertPersistentDataToTemporaryData();
            }

            _prototypeRenderData.Clear();
        }

        public static int GetCount()
        {
            return _prototypeRenderData.Count;
        }

        public static void Clear()
        {
            _prototypeRenderData.Clear();
        }
    }
}