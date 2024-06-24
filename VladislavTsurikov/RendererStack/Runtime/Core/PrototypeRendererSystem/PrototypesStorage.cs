using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    [LocationAsset("RendererStack/PrototypesStorage")]
    public class PrototypesStorage : SerializedScriptableObjectSingleton<PrototypesStorage>
    {
        [OdinSerialize]
        public List<PrototypesRendererStorage> PrototypeRendererStacks = new List<PrototypesRendererStorage>();
        
        [OnDeserializing]
        private void OnDeserializing()
        {
            PrototypeRendererStacks ??= new List<PrototypesRendererStorage>();
        }

        public void DeleteInvalidPrototypes()
        {
            foreach (var item in PrototypeRendererStacks)
            {
                item.DeleteInvalidPrototypes();
            }
        }

        public Prototype GeneratePrototypeIfNecessary(UnityEngine.Object obj, Type prototypeType, Type rendererType)
        {
            PrototypesRendererStorage prototypesRendererStorage = GetPrototypeRendererStack(rendererType);
            
            if (prototypesRendererStorage == null)
            {
                prototypesRendererStorage = new PrototypesRendererStorage(rendererType);
                PrototypeRendererStacks.Add(prototypesRendererStorage);
            }

            return prototypesRendererStorage.GeneratePrototypeIfNecessary(obj, prototypeType);
        }
        
        public void Remove(List<Prototype> protoList, Type rendererType)
        {
            PrototypesRendererStorage prototypesRendererStorage = GetPrototypeRendererStack(rendererType);
            
            if (prototypesRendererStorage != null)
            {
                prototypesRendererStorage.Remove(protoList);
            }
        }

        public Prototype GetPrototype(UnityEngine.Object obj, Type rendererType)
        {
            PrototypesRendererStorage prototypesRendererStorage = GetPrototypeRendererStack(rendererType);
            
            if (prototypesRendererStorage != null)
            {
                return prototypesRendererStorage.GetPrototype(obj);
            }

            return null;
        }
        
        public Prototype GetPrototype(int id, Type rendererType)
        {
            PrototypesRendererStorage prototypesRendererStorage = GetPrototypeRendererStack(rendererType);
            
            if (prototypesRendererStorage != null)
            {
                return prototypesRendererStorage.GetPrototype(id);
            }

            return null;
        }
        
        public bool HasPrototype(Prototype findProto, Type rendererType)
        {
            PrototypesRendererStorage prototypesRendererStorage = GetPrototypeRendererStack(rendererType);
            
            if (prototypesRendererStorage != null)
            {
                return prototypesRendererStorage.HasPrototype(findProto);
            }

            return false;
        }

        private PrototypesRendererStorage GetPrototypeRendererStack(Type rendererType)
        {
            foreach (var item in PrototypeRendererStacks)
            {
                if (item.RendererType == rendererType)
                    return item;
            }

            return null;
        }
    }
}