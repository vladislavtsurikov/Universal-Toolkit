using System;
using System.Collections.Generic;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem.Utility;
#if BillboardSystem
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.RendererData.BillboardSystem;
#endif

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.RendererDataSystem
{
    [Serializable]
    public class PrototypeRendererData
    {
        [OdinSerialize]
        private int _prototypeID;

        [NonSerialized]
        public TemporaryInstanceData TemporaryInstanceData = new TemporaryInstanceData();
        
#if BillboardSystem
        [NonSerialized]
        public BillboardInstance BillboardInstance = new BillboardInstance();
#endif

        public int PrototypeID => _prototypeID;

        [OdinSerialize]
        public List<RendererInstance> InstanceList = new List<RendererInstance>();
        
        public PrototypeRendererData(int prototypeID)
        {
            _prototypeID = prototypeID;
        }

        public void Setup()
        {
            TemporaryInstanceData = new TemporaryInstanceData();
#if BillboardSystem
            BillboardInstance = new BillboardInstance();
#endif
            ConvertPersistentDataToTemporaryData();
        }

        public void ConvertPersistentDataToTemporaryData()
        {
            if(TemporaryInstanceData == null)
            {
                TemporaryInstanceData = new TemporaryInstanceData();
            }
            
            TemporaryInstanceData.ConvertPersistentDataToTemporaryData(InstanceList); 
#if BillboardSystem
            BillboardInstance.Setup(ID, this);
#endif
        }

        public void AddPersistentData(TerrainObjectInstance instance)  
        {
            RendererInstance item;
            item.ID = instance.ID;
            item.Position = instance.Position;
            item.Rotation = instance.Rotation;
            item.Scale = instance.Scale;
            InstanceList.Add(item);
            
            ModifiedPrototypeRenderDataStack.Add(this);
        }

        public void ClearPersistentData()
        {
            InstanceList.Clear();
            ModifiedPrototypeRenderDataStack.Add(this);
        }

        public void RemovePersistentInstance(TerrainObjectInstance instance)
        {
            for (int i = 0; i < InstanceList.Count; i++)
            {
                if(InstanceList[i].ID == instance.ID)
                {
                    InstanceList.RemoveAt(i);
                    ModifiedPrototypeRenderDataStack.Add(this);
                    return;
                }
            }
        }

        public void SyncPersistentRendererInstance(TerrainObjectInstance instance)
        {
            for (int i = 0; i < InstanceList.Count; i++)
            {
                if(InstanceList[i].ID == instance.ID)
                {
                    InstanceList[i] = RendererInstance.ConvertToSerializableInstance(instance);
                    ModifiedPrototypeRenderDataStack.Add(this);
                    return;
                }
            }
        }

        public void SetEnable(TerrainObjectInstance instance)
        {
            for (int i = 0; i < InstanceList.Count; i++)
            {
                if(InstanceList[i].ID == instance.ID)
                {
                    if(instance.Enable)
                    {
                        InstanceList.Add(RendererInstance.ConvertToSerializableInstance(instance));
                    }
                    else
                    {
                        InstanceList.RemoveAt(i);
                    }

                    ModifiedPrototypeRenderDataStack.Add(this);
                    return;
                }
            }
        }

        public void OnDisable()
        {
            TemporaryInstanceData?.DisposeUnmanagedMemory();
            
#if BillboardSystem
            BillboardInstance?.Dispose();
#endif
        }
    }
}