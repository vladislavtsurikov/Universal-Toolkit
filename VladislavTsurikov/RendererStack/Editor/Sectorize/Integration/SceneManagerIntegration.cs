#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Core.Runtime.Interfaces;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility.Utility;
using VladislavTsurikov.SceneManagerTool.Editor;
using VladislavTsurikov.SceneManagerTool.Runtime;
using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem.Components;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.Integration
{
    internal static class SceneManagerIntegration
    {
        internal static void PrepareSceneManager()
        {
            List<Sector> sectors =
                StreamingUtility.GetAllScenes(Runtime.Sectorize.Sectorize.GetSectorLayerTag());

            if (sectors.Count == 0) return;

            if (SceneManagerData.Instance.EnableSceneManager == false)
            {
                EnableSceneManagerWithWarning();
            }

            if (SceneManagerData.Instance.EnableSceneManager)
            {
                Scene scene = SectorLayerManager.Instance.SceneDataManager.Scene;
            
                Profile profile = SceneManagerData.Instance.Profile;

                if (!profile)
                {
                    SceneManagerData.Instance.Profile = Profile.CreateProfile();
                }

                DefaultBuildSceneCollection buildSceneCollection = GetCurrentDefaultBuildSceneCollection();

                SceneCollection sceneCollection = GetSectorizeSceneCollection(buildSceneCollection);

                ActiveScene activeSceneSettings = (ActiveScene)sceneCollection.SettingsList.CreateIfMissingType(typeof(ActiveScene));
                activeSceneSettings.SceneReference = new SceneReference(scene);

                Runtime.Sectorize.SceneManagerIntegration.Sectorize sectorizeComponent = (Runtime.Sectorize.SceneManagerIntegration.Sectorize)sceneCollection.SceneComponentStack.CreateComponent(
                    typeof(Runtime.Sectorize.SceneManagerIntegration.Sectorize));
            
                foreach (var sector in StreamingUtility.GetAllScenes(Runtime.Sectorize.Sectorize.GetSectorLayerTag()))
                {
                    sectorizeComponent.AddSubScene(sector.SceneReference);
                }
            }
        }

        internal static DefaultBuildSceneCollection GetCurrentDefaultBuildSceneCollection()
        {
            Profile profile = SceneManagerData.Instance.Profile;
            
            if (profile.BuildSceneCollectionList.ElementList.Count == 0)
            {
                return (DefaultBuildSceneCollection)profile.BuildSceneCollectionList.CreateComponent(typeof(DefaultBuildSceneCollection));
            }
            else
            {
                if (profile.BuildSceneCollectionList.ActiveBuildSceneCollection is DefaultBuildSceneCollection)
                {
                    return (DefaultBuildSceneCollection)profile.BuildSceneCollectionList.ActiveBuildSceneCollection;
                }
                else
                {
                    DefaultBuildSceneCollection buildSceneCollection = (DefaultBuildSceneCollection)profile.BuildSceneCollectionList.CreateComponent(typeof(DefaultBuildSceneCollection));
                    profile.BuildSceneCollectionList.ActiveBuildSceneCollection = buildSceneCollection;
                    return buildSceneCollection;
                }
            }
        }
        
        private static SceneCollection GetSectorizeSceneCollection(DefaultBuildSceneCollection buildSceneCollection)
        {
            Scene scene = SectorLayerManager.Instance.SceneDataManager.Scene;

            foreach (var sceneCollection in buildSceneCollection.SceneCollectionList.ElementList)
            {
                if (((IHasName)sceneCollection).Name == scene.name)
                {
                    return sceneCollection;
                }
            }
            
            SceneCollection newSceneCollection = buildSceneCollection.SceneCollectionList.CreateComponent(typeof(SceneCollection));
            newSceneCollection.Name = scene.name;

            return newSceneCollection;
        }

        public static void EnableSceneManagerWithWarning()
        {
            if (EditorUtility.DisplayDialog("WARNING!",
                    "Sectorize works with the Scene Manager. If you enable the Scene Manager, this will remove your added scenes in the Build Settings and automatically add only the added scenes to the Scene Manager in the Build Settings.", 
                    "OK", "Cancel"))
            {
                SceneManagerData.Instance.EnableSceneManager = true;
                SceneManagerWindow.Open();
            }
        }
    }
}
#endif