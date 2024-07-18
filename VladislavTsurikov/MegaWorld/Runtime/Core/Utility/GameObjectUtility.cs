using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.RendererStack.Runtime.Sectorize;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.Utility
{
    public static class GameObjectUtility
    {
        public static GameObject Instantiate(GameObject prefab, Vector3 position, Vector3 scaleFactor, Quaternion rotation)
        {
#if UNITY_EDITOR
            var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
            var go = Object.Instantiate(prefab);
#endif

            SceneDataManager sceneDataManager = SceneDataManagerFinder.OverlapPosition(position, Sectorize.GetSectorLayerTag(), false)[0];
            SceneManager.MoveGameObjectToScene(go, sceneDataManager.Scene);

            go.transform.position = position;
            go.transform.localScale = scaleFactor;
            go.transform.rotation = rotation;

            return go;
        }
    }
}