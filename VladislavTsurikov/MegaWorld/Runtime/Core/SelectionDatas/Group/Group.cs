using System;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility;
using VladislavTsurikov.ReflectionUtility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group
{
    public class Group : SerializedScriptableObject, IHasElementStack, IShowIcon
    {
        [OdinSerialize] 
        private bool _selected;
        [OdinSerialize] 
        private Type _prototypeType;
        
        [OdinSerialize]
        private ComponentStackManager _componentStackManager;

        [OdinSerialize] 
        private DefaultGroupComponentStack _defaultGroupComponentStack;

        public DefaultGroupComponentStack DefaultGroupComponentStack => _defaultGroupComponentStack;

        public Type PrototypeType => _prototypeType;
        
        [OdinSerialize] 
        public AdvancedElementList<Prototype> PrototypeList = new AdvancedElementList<Prototype>();

        public string RenamingName = "Group";
        public bool Renaming;
        public string Name => name;

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }
        
        public ComponentStackManager ComponentStackManager => _componentStackManager;

#if UNITY_EDITOR
        public bool IsRedIcon => false;
        public Texture2D PreviewTexture => null;
#endif

        public void OnEnable() 
        {
            AllAvailableGroups.AddGroup(this);
            
            PrototypeList.OnRemoved -= OnRemove;
            PrototypeList.OnRemoved += OnRemove;

            if (_prototypeType != null)
            {
                SetupComponentStack();
                SetupPrototypesElementStack();
            }

            foreach (var prototype in PrototypeList)
            {
                prototype.Setup();
            }
        }

        private void OnRemove(int index)
        {
            PrototypeList[index].OnDisable();
        }

        private void OnDestroy()
        {
            AllAvailableGroups.RemoveGroup(this);
            
            foreach (var prototype in PrototypeList)
            {
                prototype.OnDisable();
            }
        }

        internal void Init(Type prototypeType)
        {
            _prototypeType = prototypeType;
            
            SetupComponentStack();
            SetupPrototypesElementStack();
        }
        
        public void SetupComponentStack()
        {
            _componentStackManager ??= new ComponentStackManager();
            _defaultGroupComponentStack ??= new DefaultGroupComponentStack();

            _componentStackManager.SetupElementStack(typeof(AddGeneralGroupComponentsAttribute),
                typeof(AddGroupComponentsAttribute), PrototypeType);
            _defaultGroupComponentStack.Setup(true, new object[]{this});
        }

        public void SetupPrototypesElementStack()
        {
            foreach (var prototype in PrototypeList)
            {
                prototype.SetupComponentStack();
            }
        }

        public Runtime_Core_Component GetElement(Type elementType)
        {
            return _componentStackManager.GeneralComponentStack.GetElement(elementType);
        }

        public Runtime_Core_Component GetElement(Type toolType, Type elementType)
        {
            return _componentStackManager.ToolsComponentStack.GetElement(toolType, elementType);
        }
        
        public T GetDefaultElement<T>() where T: DefaultGroupComponent
        {
            return (T)_defaultGroupComponentStack.GetElement(typeof(T));
        }
        
        public Prototype AddMissingPrototype(UnityEngine.Object obj)
        {
            return AddMissingPrototype(GeneratePrototypeIfNecessary(obj));
        }

        private Prototype AddMissingPrototype(Prototype proto)
        {
            if (proto == null || proto.PrototypeObject == null)
            {
                return null;
            }

            if (GetPrototype(proto.ID) != null)
            {
                return proto;
            }
            
            PrototypeList.Add(proto);

            return proto;
        }

        private Prototype GeneratePrototypeIfNecessary(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("You are adding a null object " + "(" + PrototypeType + ")");
                return null;
            }
            
            Prototype prototype = GetPrototype(obj);
            
            int id = obj.GetInstanceID();

            if (prototype == null)
            {
                prototype = (Prototype)Activator.CreateInstance(PrototypeType);
                prototype.OnCreate(id, obj);
                PrototypeList.Add(prototype);

#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }

            return prototype;
        }

        public Prototype GetPrototype(UnityEngine.Object obj)
        {
            foreach (Prototype proto in PrototypeList)
            {
                if(proto.PrototypeObject == obj)
                {
                    return proto;
                }
            }

            return null;
        }
        
        public Prototype GetPrototype(int id)
        {
            foreach (var proto in PrototypeList)
            {
                if (proto.ID == id)
                {
                    return proto;
                }
            }

            return null;
        }
        
        public List<Prototype> GetAllSelectedPrototypes()
        {
            List<Prototype> protoList = new List<Prototype>();

            foreach (var proto in PrototypeList)
            {
                if(proto.Selected)
                {
                    protoList.Add(proto);
                }
            }

            return protoList;
        }
        
        public bool HasAllActivePrototypes()
        {
            foreach (Prototype proto in PrototypeList)
            {
                if(proto.Active)
                {
                    return true;
                }
            }

            return false;
        }
        
        public void DeleteSelectedPrototype()
        {
            PrototypeList.RemoveAll(proto => proto.Selected);
        }

        public void SelectAllPrototypes(bool select)
        {
            PrototypeList.ForEach(proto => proto.Selected = select);
        }

#if UNITY_EDITOR
        public string GetPrototypeTypeName()
        {
            return PrototypeType.GetAttribute<NameAttribute>().Name.Split('/').Last();
        }
        
        public void Save() 
        {
            EditorUtility.SetDirty(this);
        }
        
        // Sadly OnDestroy is not being called reliably by the editor. So we need this.
        private class OnDestroyProcessor: AssetModificationProcessor
        {
            static Type _type = typeof(Group);
     
            static string _fileEnding = ".asset";
 
            public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions _)
            {
                if (!path.EndsWith(_fileEnding))
                {
                    return AssetDeleteResult.DidNotDelete;
                }
 
                var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (assetType != null && (assetType == _type || assetType.IsSubclassOf(_type)))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Group>(path);
                    asset.OnDestroy();
                }
 
                return AssetDeleteResult.DidNotDelete;
            }
        }
#endif
    }
}