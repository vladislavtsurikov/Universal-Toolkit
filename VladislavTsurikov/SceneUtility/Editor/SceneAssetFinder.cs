﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VladislavTsurikov.SceneUtility.Editor
{
    public static class SceneAssetFinder
    {
        public static SceneAsset FindSceneAsset(Scene scene)
        {
            string scenePath = scene.path;
            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogError("Scene is not saved.");
                return null;
            }

            string assetGuid = AssetDatabase.AssetPathToGUID(scenePath);
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(assetGuid));
            return sceneAsset;
        }
    }
}
#endif