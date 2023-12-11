#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.GameObjectCollider.Runtime.Utility;

namespace VladislavTsurikov.Undo.Editor.UndoActions
{
    public class CreatedGameObject : UndoRecord
    {
        private readonly List<GameObject> _gameObjectList = new List<GameObject>();

        public CreatedGameObject(GameObject gameObject) 
        {
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
            
            _gameObjectList.Add(prefabRoot);
        }

        public override void Merge(UndoRecord record)
        {
            if (!(record is CreatedGameObject)) return;
            CreatedGameObject gameObjectUndo = (CreatedGameObject)record;
            _gameObjectList.AddRange(gameObjectUndo._gameObjectList);
        }

        public override void Undo()
        {
            foreach (var gameObject in _gameObjectList)
            {
                Object.DestroyImmediate(gameObject);
            }

            GameObjectColliderUtility.RemoveNullObjectNodesForAllScenes();
        }
    }
}
#endif