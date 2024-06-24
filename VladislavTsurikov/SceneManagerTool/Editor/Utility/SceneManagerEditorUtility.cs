#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.SceneManagerTool.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    public static class SceneManagerEditorUtility
    {
        public static void SetAllScenesToDirty()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }
        
        public static void EnterPlaymode()
        {
            SceneManagerData.MaskAsDirty();
            
            CoroutineRunner.StartCoroutine(Coroutine(), SceneManagerData.Instance); 
            
            return;

            IEnumerator Coroutine()
            {
                while (EditorApplication.isCompiling)
                    yield return null;
                
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                
                SceneManagerData.Instance.SceneManagerEditorData.SceneSetupManager.SaveSceneSetup();
                StartupScene.Open();
                
                yield return new WaitForSeconds(0.1f);

                EditorApplication.EnterPlaymode();
                SceneManagerData.Instance.SceneManagerEditorData.RunAsBuildMode = true;
            }
        }
    }
}
#endif