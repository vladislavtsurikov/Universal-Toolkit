#if UNITY_EDITOR
using System.Collections.Generic;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;

namespace VladislavTsurikov.Undo.Editor.Actions.TerrainObjectRenderer
{
    public class TerrainObjectTransform : UndoRecord
    {
        private readonly List<TransformData> _transformList = new List<TransformData>();

        public TerrainObjectTransform(TerrainObjectInstance gameObject) 
        {
            _transformList.Add(new TransformData(gameObject));
        }

        public override void Merge(UndoRecord record)
        {
            if (record is TerrainObjectTransform gameObjectUndo)
            {
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

        private class TransformData
        {
            private readonly TerrainObjectInstance _gameObject;
            private readonly Transform _transform;

            public TransformData(TerrainObjectInstance gameObject)
            {
                _gameObject = gameObject;
                _transform = new Transform(gameObject.Position, gameObject.Scale, gameObject.Rotation);
            }

            public void SetTransform()
            {
                if (_gameObject == null || _gameObject.IsDestroy)
                {
                    return;
                }
            
                _gameObject.Position = _transform.Position;
                _gameObject.Scale = _transform.Scale; 
                _gameObject.Rotation = _transform.Rotation;
            }
        }
    }
}
#endif