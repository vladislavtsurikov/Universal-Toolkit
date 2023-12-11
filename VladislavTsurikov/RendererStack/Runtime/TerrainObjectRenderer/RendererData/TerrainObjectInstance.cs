using System;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.DeepCopy.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData.ColliderSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.MonoBehaviour;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Components.Scripting;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData
{
    public class TerrainObjectInstance : Instance
    {
        private bool _enable = true;
        private readonly ComponentStackOnlyDifferentTypes<Script> _scriptList = new ComponentStackOnlyDifferentTypes<Script>();

        private PrototypeTerrainObject _proto;
        
        public readonly int PrototypeID;

        public PrototypeTerrainObject Proto
        {
            get
            {
                if(_proto == null)
                {
                    _proto = (PrototypeTerrainObject)TerrainObjectRenderer.Instance.SelectionData.GetProto(PrototypeID);
                }
                return _proto;
            }
        }

        [NonSerialized] 
        public TerrainObjectCollider TerrainObjectCollider;
        [NonSerialized] 
        public HierarchyTerrainObjectInstance HierarchyTerrainObjectInstance;
        
        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                
                _enable = value;

                TerrainObjectCollider.PathToTerrainObjectCollider.PrototypeRendererData.SetEnable(this);
            }
        }

        public TerrainObjectInstance(Vector3 position, Vector3 scale, Quaternion rotation, PrototypeTerrainObject proto) : base(position, scale, rotation)
        {
            _proto = proto;
            PrototypeID = proto.ID;
            _scriptList.Setup(true, this);
        }
        
        public TerrainObjectInstance(int id, Vector3 position, Vector3 scale, Quaternion rotation, PrototypeTerrainObject proto) : base(id, position, scale, rotation)
        {
            _proto = proto;
            PrototypeID = proto.ID;
            _scriptList.Setup(true, this);
        }

        public bool HasScriptType(Type type)
        {
            return GetScript(type) != null;
        }
        
        public void AddScript(Script protoScript)
        {
            if (!HasScriptType(protoScript.GetType()))
            {
                var script = DeepCopier.Copy(protoScript);
                _scriptList.AddIfMissingType(script);
            }
        }

        public void SetupScripts()
        {
            _scriptList.Setup();
        }

        public Script[] GetScripts()
        {
            return _scriptList.ElementList.ToArray();
        }
        
        public Script GetScript(Type type)
        {
            return _scriptList.GetElement(type);
        }

        public override void Destroy()
        {
            PathToTerrainObjectCollider pathToTerrainObjectCollider = TerrainObjectCollider.PathToTerrainObjectCollider;
            
            pathToTerrainObjectCollider.BVHObjectTree.Tree.RemoveLeafNode(pathToTerrainObjectCollider.LeafNode);
            pathToTerrainObjectCollider.PrototypeRendererData.RemovePersistentInstance(this);

            pathToTerrainObjectCollider.ColliderCell.ChangeNodeSizeIfNecessary(TerrainObjectCollider);
            pathToTerrainObjectCollider.RenderCell.ChangeNodeSizeIfNecessary(TerrainObjectCollider);
            
            SceneObjectsBoundsUtility.ChangeSceneObjectsBounds(pathToTerrainObjectCollider.SceneDataManager.Sector);

            if (HierarchyTerrainObjectInstance != null)
            {
                ScriptingSystem.ScriptingSystem.RemoveCollider(this);
            }
        }

        protected override void TransformChanged()
        {
            PathToTerrainObjectCollider pathToTerrainObjectCollider = TerrainObjectCollider.PathToTerrainObjectCollider;
            
            pathToTerrainObjectCollider.BVHObjectTree.Tree.RemoveLeafNode(pathToTerrainObjectCollider.LeafNode);
            
            pathToTerrainObjectCollider.PrototypeRendererData.RemovePersistentInstance(this);

            TerrainObjectRendererData.AddInstance(this, Sectorize.Sectorize.GetSectorLayerTag());
            
            if (HierarchyTerrainObjectInstance != null)
            {
                var gameObject = HierarchyTerrainObjectInstance.gameObject;
                
                gameObject.transform.position = Position;
                gameObject.transform.rotation = Rotation;
                gameObject.transform.localScale = Scale;
            }
        }

        protected override void Reposition()
        {
            PathToTerrainObjectCollider pathToTerrainObjectCollider = TerrainObjectCollider.PathToTerrainObjectCollider;
            
            pathToTerrainObjectCollider.BVHObjectTree.Tree.RemoveLeafNode(pathToTerrainObjectCollider.LeafNode);
            
            pathToTerrainObjectCollider.PrototypeRendererData.RemovePersistentInstance(this);

            TerrainObjectRendererData.AddInstance(this, Sectorize.Sectorize.GetSectorLayerTag());
            
            if (HierarchyTerrainObjectInstance != null)
            {
                var gameObject = HierarchyTerrainObjectInstance.gameObject;
                
                gameObject.transform.position = Position;
            }
        }

        protected override Vector3 GetMultiplySize()
        {
            return Proto.RenderModel.MultiplySize;
        }
    }
}