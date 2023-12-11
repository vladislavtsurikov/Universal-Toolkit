using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Attributes;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components
{
    [MenuItem("Active Scene")]
    [SceneCollection]
    public class ActiveScene : SettingsComponentElement
    {
        public SceneReference SceneReference = new SceneReference();
        
        public IEnumerator LoadScene()
        {
            yield return SceneReference.LoadScene();
            SceneManager.SetActiveScene(SceneReference.Scene);
        }
        
        public IEnumerator UnloadScene()
        {
            yield return SceneReference.UnloadScene();
        }
        
        public override List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>{SceneReference};
        }
        
        internal static IEnumerator LoadActiveSceneIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponentElement> settingsList)
        {
            ActiveScene activeScene = (ActiveScene)settingsList.GetElement(typeof(ActiveScene));

            if (activeScene != null)
            {
                yield return activeScene.LoadScene();
            }
        }
        
        internal static IEnumerator UnloadActiveSceneIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponentElement> settingsList)
        {
            ActiveScene activeScene = (ActiveScene)settingsList.GetElement(typeof(ActiveScene));

            if (activeScene != null)
            {
                yield return activeScene.UnloadScene();
            }
        }
    }
}