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
    [SceneCollectionComponent]
    public class ActiveScene : SettingsComponent
    {
        public SceneReference SceneReference = new SceneReference();
        
        internal static IEnumerator LoadActiveSceneIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            ActiveScene activeScene = (ActiveScene)settingsList.GetElement(typeof(ActiveScene));

            if (activeScene != null)
            {
                yield return activeScene.LoadScene();
            }
        }
        
        internal static IEnumerator UnloadActiveSceneIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            ActiveScene activeScene = (ActiveScene)settingsList.GetElement(typeof(ActiveScene));

            if (activeScene != null)
            {
                yield return activeScene.UnloadScene();
            }
        }
        
        public override List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>{SceneReference};
        }

        private IEnumerator LoadScene()
        {
            yield return SceneReference.LoadScene();
            SceneManager.SetActiveScene(SceneReference.Scene);
        }

        private IEnumerator UnloadScene()
        {
            yield return SceneReference.UnloadScene();
        }
    }
}