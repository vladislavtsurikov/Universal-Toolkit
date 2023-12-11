using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Core.Runtime.Interfaces;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas
{
    [Serializable]
    public class Group : IHasName, ISelected
    {
        [SerializeField]
        private bool _selected;
        [SerializeField]
        private string _name;

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }

        [OdinSerialize]
        public List<Prototype> PrototypeList = new List<Prototype>();

        public string Name
        {
            get => _name;
            set => _name = value;
        }
        
        public void Setup(Type rendererType)
        {
            SyncsReferencesWithAllPrototypeStack(rendererType);

            foreach (var prototype in PrototypeList)
            {
                prototype.Setup();
            }
        }
        
        public void SyncsReferencesWithAllPrototypeStack(Type rendererType)
        {
            for (int i = 0; i < PrototypeList.Count; i++)
            {
                Prototype proto = PrototypesStorage.Instance.GetPrototype(PrototypeList[i].ID, rendererType);
                if (proto != null)
                {
                    PrototypeList[i] = proto;
                }
            }
        }

        public Group(string name)
        {
            Name = name; 
        }

        public Group(string name, bool selected)
        {
            Name = name; 
            _selected = selected;
        }

        public void SelectAllPrototypes()
        {
            PrototypeList.ForEach(proto => proto.Selected = true);
        }
        
        internal bool DeleteInvalidPrototypes(Type rendererType)
        {
            bool happenedDelete = false;
            
            for (int i = PrototypeList.Count - 1; i >= 0; i--)
            {
                if (PrototypeList[i] == null || !PrototypesStorage.Instance.HasPrototype(PrototypeList[i], rendererType))   
                {
                    PrototypeList.RemoveAt(i);
                    happenedDelete = true;
                }
            }

            return happenedDelete;
        }
    }
}