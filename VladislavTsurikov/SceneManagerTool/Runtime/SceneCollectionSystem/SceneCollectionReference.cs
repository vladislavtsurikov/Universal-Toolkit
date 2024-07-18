using System;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.SceneManagerTool.Runtime.Utility;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem
{
    [Serializable]
    public class SceneCollectionReference
    {
        private SceneCollection _sceneCollection;
        
        [OdinSerialize]
        private int _sceneCollectionID;

        public SceneCollection SceneCollection
        {
            get
            {
                if (_sceneCollection == null)
                {
                    _sceneCollection = SceneCollectionUtility.GetSceneCollection(_sceneCollectionID);
                }

                return _sceneCollection;
            }
            set
            {
                if (value != null)
                {
                    _sceneCollection = value;
                    _sceneCollectionID = value.ID;
                }
            }
        }

        public bool IsValid()
        {
            return SceneCollection != null;
        }

        public SceneCollectionReference()
        {
            
        }

        public SceneCollectionReference(SceneCollection sceneCollection)
        {
            _sceneCollectionID = sceneCollection.ID;
            _sceneCollection = sceneCollection;
        }
    }
}