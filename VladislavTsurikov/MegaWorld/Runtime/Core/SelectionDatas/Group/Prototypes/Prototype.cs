using System;
using UnityEngine;
using VladislavTsurikov.Core.Runtime.IconStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem.Attributes;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using Component = VladislavTsurikov.ComponentStack.Runtime.Component;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes
{
    [Serializable]
    public abstract class Prototype : IHasElementStack, IShowIcon
    {
        [OdinSerialize] 
        protected int _id;

        [SerializeField]
        private bool _selected;
        
        [OdinSerialize]
        private ComponentStackManager _componentStackManager;

        public virtual string Name { get; set; }
        public virtual int ID => _id;

        public bool Active = true;

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }
        
        [OdinSerialize]
        public ComponentStackManager ComponentStackManager => _componentStackManager;

        public abstract Object PrototypeObject { get; }

#if UNITY_EDITOR
        public abstract Texture2D PreviewTexture { get; }
        public bool IsRedIcon => !Active;
#endif
        
        public void SetupComponentStack()
        {
            if (_componentStackManager == null)
            {
                _componentStackManager = new ComponentStackManager();
            }
            
            _componentStackManager.SetupElementStack(typeof(AddGeneralPrototypeComponentsAttribute),
                typeof(AddPrototypeComponentsAttribute), GetType());
        }
        
        public Component GetElement(Type elementType)
        {
            return _componentStackManager.GeneralComponentStack.GetElement(elementType);
        }

        public Component GetElement(Type toolType, Type elementType)
        {
            return _componentStackManager.ToolsComponentStack.GetElement(toolType, elementType);
        }

        internal void InitInternal(int id, Object obj)
        {
            _id = id;
            Init(obj);
            
            SetupComponentStack();
        }

        public abstract void Init(Object obj);
        public abstract bool IsSamePrototypeObject(Object obj);
    }
}
