#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.RendererStack.Editor.Sectorize.TerrainSystem;
using VladislavTsurikov.SceneDataSystem.Editor.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility.Utility;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    public partial class Sectorize
    {
        public IEnumerator CreateScenesForTerrains()
        {
            List<VirtualTerrain> terrains = AllVirtualTerrainTypes.FindAll();
            
            if (terrains.Count <= 0) yield break;

            Scene activeScene = SceneManager.GetActiveScene();
            
            SceneDataManager sceneDataManager = SceneDataManagerUtility.FindSceneDataManager(activeScene);

            sceneDataManager.SceneType = SceneType.ParentScene;

            EditorSceneManager.MarkSceneDirty(activeScene);
            
            for (int i = 0; i < terrains.Count; i++)
            {
                float progress = ((float)i / (float)terrains.Count) * 100;
                
                EditorUtility.DisplayProgressBar("Sector: " + progress + "%" + " (" + i + "/" + terrains.Count + ")", "Running " + terrains[i].Target.name, progress / 100);
                
                yield return StreamingUtilityEditor.CreateScene("Terrain", terrains[i].Target.name, terrains[i].GetTerrainBounds(), new List<GameObject>(){terrains[i].Target.gameObject});
            }

            SceneDataManagerUtility.InstanceSceneDataManagerForAllScenes();

            foreach (var sector in StreamingUtility.GetAllScenes(GetSectorLayerTag()))
            {
                sector.SceneDataManager.SceneType = SceneType.Subscene;
            }

            Editor.Sectorize.Integration.SceneManagerIntegration.PrepareSceneManager();
            
            EditorUtility.ClearProgressBar();
            
            CreateScenesAfterEvent?.Invoke();
            
            EditorUtility.ClearProgressBar();
        }
    }
}
#endif