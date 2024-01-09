#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Runtime;
using Transform = VladislavTsurikov.Runtime.Transform;

namespace VladislavTsurikov.Undo.Editor.UndoActions
{
    public class TransformData
    {
        public GameObject GameObject;
        public Transform Transform;

        public TransformData(GameObject gameObject)
        {
            GameObject = gameObject;
            Transform = new Transform(gameObject);
        }

        public void SetTransform()
        {
            if(GameObject == null)
                return;
            
            GameObject.transform.position = Transform.Position;
            GameObject.transform.localScale = Transform.Scale; 
            GameObject.transform.rotation = Transform.Rotation;
        }
    }

    public class GameObjectTransform : UndoRecord
    {
        private List<TransformData> _transformList = new List<TransformData>();

        public GameObjectTransform(GameObject gameObject) 
        {
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
            
            _transformList.Add(new TransformData(prefabRoot));
        }

        public override void Merge(UndoRecord record)
        {
            if (record is GameObjectTransform)
            {
                GameObjectTransform gameObjectUndo = (GameObjectTransform)record;
                _transformList.AddRange(gameObjectUndo._transformList);
            }
        }

        public override void Undo()
        {
            foreach (TransformData transformUndoData in _transformList)
            {
                transformUndoData?.SetTransform();
            }
        }
    }
}
#endif