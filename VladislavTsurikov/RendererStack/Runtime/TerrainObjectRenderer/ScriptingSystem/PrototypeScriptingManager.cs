using System;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.Preferences;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem.CellsInstancesSelector;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem.DefaultInstancesSelector;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.MonoBehaviour;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem
{
    public class PrototypeScriptingManager
    {
        private Transform _scriptingSystemGameObjectParent;
        
        private readonly GameObject _sourcePrototypeColliderObject;

        [NonSerialized] 
        public readonly DefaultPrototypeInstancesSelector DefaultPrototypeInstancesSelector;
        [NonSerialized] 
        public readonly CellsPrototypeInstancesSelector CellsPrototypeInstancesSelector;
        
        public readonly PrototypeTerrainObject Proto;
        
        public Colliders CollidersComponent => (Colliders)Proto.GetSettings(typeof(Colliders));
        public Scripting ScriptingComponent => (Scripting)Proto.GetSettings(typeof(Scripting));
        
        public PrototypeScriptingManager(PrototypeTerrainObject proto, Transform parent)
        {
            Proto = proto;
            
            CreateScriptingSystemGameObjectParent(proto, parent);
            
            _sourcePrototypeColliderObject = CreateSourceColliderGameObject();
            
            DefaultPrototypeInstancesSelector = new DefaultPrototypeInstancesSelector(this, proto);
            CellsPrototypeInstancesSelector = new CellsPrototypeInstancesSelector(this, proto);
        }

        public void OnDisable()
        {
            if (Application.isPlaying)
            {
                Object.Destroy(_sourcePrototypeColliderObject);
            }
            else
            {
                Object.DestroyImmediate(_sourcePrototypeColliderObject);  
            }

            DestroyScriptingSystemGameObjectParent();
        }
        
        public void SetColliders(Sphere sphere, object usedObj)
        {
            if (sphere.Radius >= 50)
            {
                DefaultPrototypeInstancesSelector.OnDisableCollider(usedObj);
                CellPrototypeInstancesSelectorData cellPrototypeInstancesSelectorData = (CellPrototypeInstancesSelectorData)CellsPrototypeInstancesSelector.GetPrototypeInstancesSelectorData(usedObj);

                if (cellPrototypeInstancesSelectorData.IsNeedUpdate(sphere))
                {
                    CellsPrototypeInstancesSelector.SetColliders(sphere, usedObj);
                }
            }
            else
            {
                CellsPrototypeInstancesSelector.OnDisableCollider(usedObj);
                DefaultPrototypeInstancesSelectorData defaultPrototypeInstancesSelectorData = (DefaultPrototypeInstancesSelectorData)DefaultPrototypeInstancesSelector.GetPrototypeInstancesSelectorData(usedObj);

                if (defaultPrototypeInstancesSelectorData.IsNeedUpdate(sphere))
                {
                    DefaultPrototypeInstancesSelector.SetColliders(sphere, usedObj);
                }
            }
        }

        private void CreateScriptingSystemGameObjectParent(PrototypeTerrainObject proto, Transform parent)
        {
            GameObject parentObject = new GameObject(proto.Prefab.name) {hideFlags = HideFlags.DontSave};
            GameObjectUtility.ParentGameObject(parentObject, parent.gameObject);
            _scriptingSystemGameObjectParent = parentObject.transform;
        }

        private void DestroyScriptingSystemGameObjectParent()
        {
            if (!_scriptingSystemGameObjectParent)
            {
                return;
            }
            
            if (Application.isPlaying)
            {
                Object.Destroy(_scriptingSystemGameObjectParent.gameObject);
            }
            else
            {
                Object.DestroyImmediate(_scriptingSystemGameObjectParent.gameObject);
            }
        }

        private GameObject CreateSourceColliderGameObject()
        {
            GameObject sourceColliderObject = new GameObject("SourceColliderObject") {hideFlags = HideFlags.DontSave};
            sourceColliderObject.transform.SetParent(_scriptingSystemGameObjectParent);
            sourceColliderObject.transform.position = Vector3.zero;
            sourceColliderObject.transform.localScale = Vector3.one;
            sourceColliderObject.transform.rotation = Quaternion.identity;
            sourceColliderObject.layer = Proto.Prefab.layer;
            sourceColliderObject.tag = Proto.Prefab.tag;
            sourceColliderObject.SetActive(false);

            if (PrototypeComponent.IsValid(CollidersComponent))
            {
                GameObject tmpColliderObject = Object.Instantiate(Proto.Prefab, _scriptingSystemGameObjectParent, true);
                tmpColliderObject.hideFlags = HideFlags.DontSave;
                tmpColliderObject.transform.position = Vector3.zero;
                tmpColliderObject.transform.localScale = Vector3.one;
                tmpColliderObject.transform.rotation = Quaternion.identity;
				
                Collider[] colliders = tmpColliderObject.GetComponentsInChildren<Collider>();

                foreach (var colliderComponent in colliders)
                {
                    string colliderObjectName = colliderComponent.GetType().ToString().Split('.').Last();
				
                    GameObject colliderObject = new GameObject(colliderObjectName) {hideFlags = HideFlags.DontSave};			
                    colliderObject.transform.SetParent(sourceColliderObject.transform);
                    var transform = colliderComponent.transform;
                    colliderObject.transform.position = transform.position;
                    colliderObject.transform.localScale = transform.localScale;
                    colliderObject.transform.rotation = transform.rotation;
                    colliderObject.layer = Proto.Prefab.layer;
                    colliderObject.tag = Proto.Prefab.tag;

                    GameObjectUtility.CopyComponent(colliderComponent, colliderObject);
                }
				
                Object.DestroyImmediate(tmpColliderObject);
            }

            return sourceColliderObject;
        }

        public void MoveGameObject(TerrainObjectInstance instance, GameObject go)
        {
            go.transform.position = instance.Position;
            go.transform.localScale = instance.Scale;
            go.transform.rotation = instance.Rotation;

            UpdateGameObjectHierarchyLargeObjectInstance(go, instance);
        }

        private void UpdateGameObjectHierarchyLargeObjectInstance(GameObject go, TerrainObjectInstance instance)
        {
            var hierarchyLargeObjectInstance = go.GetComponent<HierarchyTerrainObjectInstance>();

            if (!hierarchyLargeObjectInstance)
            {
                return;
            }

            hierarchyLargeObjectInstance.Setup(instance, ScriptingComponent);
        }

        public GameObject CreateGameObject(string objText)
        {
            var newObject = Object.Instantiate(_sourcePrototypeColliderObject, _scriptingSystemGameObjectParent, true);
            newObject.name = "Hierarchy Object_" + objText;
            newObject.hideFlags = GetVisibilityHideFlags();
            newObject.tag = Proto.Prefab.tag;	
            newObject.layer = Proto.Prefab.layer;
			
            newObject.SetActive(true);
            newObject.AddComponent<HierarchyTerrainObjectInstance>();
            
            return newObject;
        }
        
        private static HideFlags GetVisibilityHideFlags()
        {
            return RendererStackSettings.Instance.ShowColliders ? HideFlags.DontSave : HideFlags.HideAndDontSave;
        }
        
        public void RemoveColliders(object usedObj)
        {
            DefaultPrototypeInstancesSelector.OnDisableCollider(usedObj); 
            CellsPrototypeInstancesSelector.OnDisableCollider(usedObj);
        }

        public float GetMaxDistance()
        {
            if (PrototypeComponent.IsValid(CollidersComponent))
            {
                return CollidersComponent.MaxDistance;
            }
            if (PrototypeComponent.IsValid(ScriptingComponent))
            {
                return ScriptingComponent.MaxDistance;
            }

            return 0;
        }
    }
}