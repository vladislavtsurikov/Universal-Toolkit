using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    [Name("Active Scene")]
    [SceneCollectionComponent]
    public class ActiveScene : SettingsComponent
    {
        public SceneReference SceneReference = new SceneReference();
        
        internal static async UniTask LoadActiveSceneIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            ActiveScene activeScene = (ActiveScene)settingsList.GetElement(typeof(ActiveScene));

            if (activeScene != null)
            {
                await activeScene.LoadScene();
            }
        }
        
        internal static async UniTask UnloadActiveSceneIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            ActiveScene activeScene = (ActiveScene)settingsList.GetElement(typeof(ActiveScene));

            if (activeScene != null)
            {
                await activeScene.UnloadScene();
            }
        }
        
        public override List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>{SceneReference};
        }

        private async UniTask LoadScene()
        {
            await SceneReference.LoadScene();
            SceneManager.SetActiveScene(SceneReference.Scene);
        }

        private async UniTask UnloadScene()
        {
            await SceneReference.UnloadScene();
        }
    }
}