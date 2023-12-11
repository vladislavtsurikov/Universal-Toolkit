using System;
using System.Collections.Generic;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem
{
    [Serializable]
    public class PrototypeRenderDataStack
    {
        [OdinSerialize]
        public List<PrototypeRendererData> PrototypeRenderDataList = new List<PrototypeRendererData>();

        public void Setup(SelectionData selectionData)
        {
            PreparePrototypeRenderCount(selectionData);
            
            foreach (PrototypeRendererData prototypeRenderData in PrototypeRenderDataList)
            {
                prototypeRenderData.Setup();
            }
        }

        public void PreparePrototypeRenderCount(SelectionData selectionData)
        {
            for (int i = selectionData.PrototypeList.Count - 1; i >= 0; i--)
            {
                if (selectionData.PrototypeList[i] == null)
                {
                    continue;
                }

                PrototypeRendererData prototypeRendererData = GetPrototypeRenderData(selectionData.PrototypeList[i].ID);

                if (prototypeRendererData == null)
                {
                    PrototypeRenderDataList.Add(new PrototypeRendererData(selectionData.PrototypeList[i].ID));
                }
            }

            for (int i = PrototypeRenderDataList.Count - 1; i >= 0; i--)
            {
                if (selectionData.GetProto(PrototypeRenderDataList[i].PrototypeID) == null)
                {
                    PrototypeRenderDataList.RemoveAt(i);
                }
                else if (PrototypeRenderDataList[i].InstanceList.Count == 0)
                {
                    PrototypeRenderDataList.RemoveAt(i);
                }
            }
        }

        public void RemoveInstances(int id)
        {
            PrototypeRendererData prototypeRendererData = GetPrototypeRenderData(id);
            if (prototypeRendererData != null)
            {
                prototypeRendererData.ClearPersistentData();
            }
        }

        public void ClearInstance()
        {          
            foreach (PrototypeRendererData prototypeRenderData in PrototypeRenderDataList)
            {
                prototypeRenderData.ClearPersistentData();
            }
        }

        public PrototypeRendererData GetPrototypeRenderData(int id)
        {
            foreach (PrototypeRendererData item in PrototypeRenderDataList)
            {
                if (item.PrototypeID == id) 
                {
                    return item;
                }
            }

            return null;
        }

        public void OnDisable()
        {
            foreach (PrototypeRendererData item in PrototypeRenderDataList)
            {
                item?.OnDisable();
            }
        }
    }
}