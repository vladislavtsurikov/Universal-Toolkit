using UnityEngine;
using UnityEngine.Windows;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.OdinSerializer.Unity_Integration.SerializedUnityObjects;

#if UNITY_EDITOR
using VladislavTsurikov.SceneManagerTool.Editor.BuildSceneCollectionSystem;
using UnityEditor;
#endif

using VladislavTsurikov.SceneManagerTool.Runtime.BuildSceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime 
{
    [CreateAssetMenu(fileName = "Profile", menuName = "SceneManager/Profile")] 
    public class Profile : SerializedScriptableObject
    { 
        [OdinSerialize] 
        public BuildSceneCollectionStack BuildSceneCollectionStack = new BuildSceneCollectionStack(); 
        
#if UNITY_EDITOR
        public BuildSceneCollectionStackEditor BuildSceneCollectionStackEditor;
#endif

        public static Profile Current => SceneManagerData.Instance.Profile;

        public void Setup()
        { 
                BuildSceneCollectionStack.Setup();
#if UNITY_EDITOR
                BuildSceneCollectionStackEditor = new BuildSceneCollectionStackEditor(BuildSceneCollectionStack);
#endif
        }

#if UNITY_EDITOR
        public void MaskAsDirty()
        { 
                EditorUtility.SetDirty(this);
        }
#endif
        public static Profile CreateProfile()
        {
#if UNITY_EDITOR
            Directory.CreateDirectory(SceneManagerPath.PathToResourcesSceneManager); 
            
            var path = string.Empty;

            path += "Profile.asset";

            path = CommonPath.CombinePath(SceneManagerPath.PathToResourcesSceneManager, path);
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            var asset = CreateInstance<Profile>(); 
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;

#else 
            return null;
#endif
        }
    }
}