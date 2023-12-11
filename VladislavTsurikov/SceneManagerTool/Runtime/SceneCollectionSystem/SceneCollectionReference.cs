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
        public int SceneCollectionID;

        public SceneCollection SceneCollection
        {
            get
            {
                if (_sceneCollection == null)
                {
                    _sceneCollection = SceneCollectionUtility.GetSceneCollection(SceneCollectionID);
                }

                return _sceneCollection;
            }
            set
            {
                if (value != null)
                {
                    _sceneCollection = value;
                    SceneCollectionID = value.ID;
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
            SceneCollectionID = sceneCollection.ID;
            _sceneCollection = sceneCollection;
        }
    }
}