using System;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Object = UnityEngine.Object;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes
{
    [Serializable]
    public abstract class Prototype : IHasElementStack, IShowIcon, IRemovable
    {
        [OdinSerialize] 
        protected int _id;

        [SerializeField]
        private bool _selected;
        
        [OdinSerialize]
        private ComponentStackManager _componentStackManager;

        public virtual string Name { get; set; }
        public int ID => _id;

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
        
        public abstract void OnCreatePrototype(Object obj);
        public abstract bool IsSamePrototypeObject(Object obj);
        
        public virtual void SetupPrototype() { }
        public virtual void OnDisablePrototype() { }
         
        internal void OnCreate(int id, Object obj)
        {
            _id = id;
            OnCreatePrototype(obj);

            Setup();
        }
        
        internal void Setup()
        {
            SetupComponentStack();
            
            AllAvailablePrototypes.AddPrototype(this);

            SetupPrototype();
        }

        internal void OnDisable()
        {
            AllAvailablePrototypes.RemovePrototype(this);
            OnDisablePrototype();
        }
        
        void IRemovable.OnRemove()
        {
            OnDisable();
        }

        public void SetupComponentStack()
        {
            if (_componentStackManager == null)
            {
                _componentStackManager = new ComponentStackManager();
            }
            
            _componentStackManager.SetupElementStack(typeof(AddGeneralPrototypeComponentsAttribute),
                typeof(AddPrototypeComponentsAttribute), GetType());
        }
        
        public Runtime_Core_Component GetElement(Type elementType)
        {
            return _componentStackManager.GeneralComponentStack.GetElement(elementType);
        }

        public Runtime_Core_Component GetElement(Type toolType, Type elementType)
        {
            return _componentStackManager.ToolsComponentStack.GetElement(toolType, elementType);
        }
    }
}
