using UnityEngine;
#if RENDERER_STACK
using VladislavTsurikov.RendererStack.Runtime.Sectorize;
#endif
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
            
            go.transform.position = position;
            go.transform.localScale = scaleFactor;
            go.transform.rotation = rotation;

            return go;
        }
    }
}