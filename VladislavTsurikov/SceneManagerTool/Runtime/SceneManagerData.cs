using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.SceneUtility.Runtime;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;
#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.SceneManagerTool.Editor;
using VladislavTsurikov.SceneUtility.Editor;
#endif

namespace VladislavTsurikov.SceneManagerTool.Runtime
{
    [LocationAsset("SceneManager/SceneManagerData")]
    public class SceneManagerData : SerializedScriptableObjectSingleton<SceneManagerData>
    {
        [OdinSerialize] 
        private Profile _profile;
        
        public bool EnableSceneManager;

        public Profile Profile
        {
#if UNITY_EDITOR
            set
            {
                if (_profile != value)
                {
                    _profile = value;
                    _profile.Setup();
                }
            }
#endif
            get => _profile;
        }

#if UNITY_EDITOR
        [OdinSerialize] 
        internal SceneManagerEditorData SceneManagerEditorData = new SceneManagerEditorData();
#endif

        private void OnEnable()
        {
            Setup();
        }

        private void OnDisable()
        {
            if (Profile == null)
            {
                return;
            }
            
            Profile.BuildSceneCollectionStack.OnDisable();
        }

        public void Setup()
        {
            if (Profile == null)
            {
                return;
            }
            
            Profile.Setup();
            
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }

            SceneReference.OnDeleteScene -= Setup;
            SceneReference.OnDeleteScene += Setup;
            
            SceneManagerEditorData.Setup();
            ScenesInBuildUtility.Setup(GetAllScenePaths());
#endif
        }

#if UNITY_EDITOR
        public static void MaskAsDirty()
        {
            EditorUtility.SetDirty(Instance);
            if (Instance._profile != null)
            {
                Instance._profile.MaskAsDirty();
            }
        }
#endif

        public static bool IsValidSceneManager()
        {
            return Instance.EnableSceneManager && Instance._profile != null;
        }

        public List<string> GetAllScenePaths()
        {
            List<string> scenePaths = new List<string>();

            foreach (var sceneReference in Profile.BuildSceneCollectionStack.GetSceneReferences())
            {
                if (sceneReference.IsValid())
                {
                    scenePaths.Add(sceneReference.ScenePath);
                }
            }
            
#if UNITY_EDITOR
            scenePaths.Add(StartupScene.GetStartupScenePath());
#endif
            
            return scenePaths;

        }
    }
}