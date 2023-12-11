#if UNITY_EDITOR
using System;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.SceneManagerTool.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    [Serializable]
    internal class SceneManagerEditorData
    {
        [OdinSerialize]
        private bool _runAsRunAsBuildMode;
        
        public bool RunAsBuildMode
        {
            get => _runAsRunAsBuildMode;
            set
            {
                _runAsRunAsBuildMode = value;
                SceneManagerData.MaskAsDirty();
            }
        }

        public SceneSetupManager SceneSetupManager = new SceneSetupManager();

        public StartupScene StartupScene = new StartupScene();

        public void Setup()
        {
            StartupScene.Setup();
            SceneSetupManager.Setup();
        }
    }
}
#endif