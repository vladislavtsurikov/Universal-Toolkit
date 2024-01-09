#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using VladislavTsurikov.Runtime;
using GameObjectUtility = VladislavTsurikov.Utility.Runtime.GameObjectUtility;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.Undo.Editor.UndoActions
{
    public class DestroyedData
    {
        public GameObject Prefab;
        public GameObject Parent;
        public Transform Transform;

        public DestroyedData(GameObject gameObject, GameObject parent)
        {
            Parent = parent;
            
            Prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            Transform = new Transform(gameObject); 
        }

        public GameObject Instantiate()
        {
            GameObject go;
            
            go = PrefabUtility.InstantiatePrefab(Prefab) as GameObject;

            go.transform.position = Transform.Position;
            go.transform.localScale = Transform.Scale;
            go.transform.rotation = Transform.Rotation;

            GameObjectUtility.ParentGameObject(go, Parent);

            return go;
        }
    }

    public class DestroyedGameObject : UndoRecord
    {
        private List<DestroyedData> _destroyDataList = new List<DestroyedData>();
        public static event UnityAction<List<GameObject>> UndoPerformed;

        public DestroyedGameObject(GameObject gameObject)
        {
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
                
            if(gameObject.transform.parent == null)
            {
                _destroyDataList.Add(new DestroyedData(prefabRoot, null));
            }
            else
            {
                _destroyDataList.Add(new DestroyedData(prefabRoot, prefabRoot.transform.parent.gameObject));
            }
        }

        public override void Merge(UndoRecord record)
        {
            if (record is DestroyedGameObject)
            {
                DestroyedGameObject gameObjectUndo = (DestroyedGameObject)record;
                _destroyDataList.AddRange(gameObjectUndo._destroyDataList);
            }
        }

        public override void Undo()
        {
            List<GameObject> gameObjectList = new List<GameObject>();

            foreach (DestroyedData destroyData in _destroyDataList)
            {
                if(destroyData.Prefab != null)
                {
                    gameObjectList.Add(destroyData.Instantiate());
                }
            }

            UndoPerformed(gameObjectList);
        }
    }
}
#endif