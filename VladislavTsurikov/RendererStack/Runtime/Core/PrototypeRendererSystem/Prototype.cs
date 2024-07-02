using System;
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Console;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    [Serializable]
    public abstract class Prototype : IShowIcon
    {       
        [OdinSerialize]
        private int _id;
        [OdinSerialize]
        private bool _selected;
        [OdinSerialize]
        private bool _active = true;
        [OdinSerialize]
        private Type _rendererType;
        private ChangeShaderCodeAttribute _changeShaderCodeAttribute;

        public int ID => _id;

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }
        
        

        public bool Active
        {
            get
            {
                if (PrototypeConsole.HasError())
                    return false;
                
                return _active;
            }
            set => _active = value;
        }
        

        public RenderModel RenderModel;
        [OdinSerialize]
        public PrototypeComponentStack PrototypeComponentStack;
        
        [NonSerialized]
        public PrototypeConsole PrototypeConsole = new PrototypeConsole();
        public abstract string Name { get; }

        public abstract Object PrototypeObject { get; }
        
        public abstract LayerMask Layer { get; }
        
        public ChangeShaderCodeAttribute ChangeShaderCodeAttribute
        {
            get
            {
                if (_changeShaderCodeAttribute == null)
                {
                    _changeShaderCodeAttribute = (ChangeShaderCodeAttribute)_rendererType.GetAttribute(typeof(ChangeShaderCodeAttribute));
                }

                return _changeShaderCodeAttribute;
            }
        }

#if UNITY_EDITOR
        public bool IsRedIcon => PrototypeConsole.HasError();
        public abstract Texture2D PreviewTexture { get; }

        private PrototypeComponentStackEditor _prototypeComponentStackEditor;
        public PrototypeComponentStackEditor PrototypeComponentStackEditor
        {
            get
            {
                if(_prototypeComponentStackEditor == null)
                {
                    _prototypeComponentStackEditor = new PrototypeComponentStackEditor(PrototypeComponentStack); 
                }
                return _prototypeComponentStackEditor;
            }
        }
#endif
        
        [OnDeserializing]
        public void OnDeserializing()
        {
            PrototypeConsole = new PrototypeConsole();
            _active = true;
        }
        
        internal void Init(int id, Object obj, Type rendererType)
        {
            _id = id;
            _rendererType = rendererType;
            InitPrototype(obj);
            PrototypeComponentStack = new PrototypeComponentStack();
            PrototypeComponentStack.Setup(true, _rendererType, this);
            PrototypeComponentStack.CreateAllComponents();
            Setup();
        }

        public void Setup()
        {
            PrototypeComponentStack.Setup(true, _rendererType, this);
            
#if UNITY_EDITOR
            ChangeShaderCodeAttribute.ChangeShaderCode(this);
#endif
            
            RefreshRenderModelInfo();
        }

        public PrototypeComponent GetSettings(Type settingsType)
        {
            return PrototypeComponentStack.GetElement(settingsType);
        }

        protected abstract void InitPrototype(Object obj);
        public abstract MeshRenderer[] GetMeshRenderers();
        public abstract bool IsSamePrototypeObject(Object obj);
        public abstract void RefreshRenderModelInfo();
    }
}
