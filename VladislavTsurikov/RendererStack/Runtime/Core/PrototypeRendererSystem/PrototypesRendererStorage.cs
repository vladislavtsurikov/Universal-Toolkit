using System;
using System.Collections.Generic;
using UnityEditor;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    [Serializable]
    public class PrototypesRendererStorage
    {
        public Type RendererType;
            
        [OdinSerialize]
        private List<Prototype> _prototypeList = new List<Prototype>();

        public PrototypesRendererStorage(Type rendererType)
        {
            RendererType = rendererType;
        }

        public void DeleteInvalidPrototypes()
        {
            List<Prototype> removePrototypeList = new List<Prototype>();
            
            foreach (Prototype prototype in _prototypeList)
            {
                if (prototype == null || prototype.PrototypeObject == null)
                {
                    removePrototypeList.Add(prototype);
                }
            }
            if (removePrototypeList.Count != 0)
            {
                Remove(removePrototypeList);
            }
        }

        public Prototype GeneratePrototypeIfNecessary(UnityEngine.Object obj, Type prototypeType)
        {
            Prototype prototype = GetPrototype(obj);
                
            int id = obj.GetInstanceID();

            if (prototype == null)
            {
                prototype = (Prototype)Activator.CreateInstance(prototypeType);
                prototype.Init(id, obj, RendererType);
                _prototypeList.Add(prototype);
                
#if UNITY_EDITOR
                EditorUtility.SetDirty(PrototypesStorage.Instance);
#endif
            }

            return prototype;
        }
        
        public void Remove(List<Prototype> protoList)
        {
            if (_prototypeList.RemoveAll(protoList.Contains) != 0)
            {
#if UNITY_EDITOR
                EditorUtility.SetDirty(PrototypesStorage.Instance);
#endif
            }
        }

        public Prototype GetPrototype(UnityEngine.Object obj)
        {
            foreach (Prototype proto in _prototypeList)
            {
                if(proto.IsSamePrototypeObject(obj))
                {
                    return proto;
                }
            }

            return null;
        }
        
        public Prototype GetPrototype(int id)
        {
            foreach (Prototype proto in _prototypeList)
            {
                if(proto.ID == id)
                {
                    return proto;
                }
            }

            return null;
        }
        
        public bool HasPrototype(Prototype findProto)
        {
            foreach (Prototype proto in _prototypeList)
            {
                if(proto.ID == findProto.ID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}