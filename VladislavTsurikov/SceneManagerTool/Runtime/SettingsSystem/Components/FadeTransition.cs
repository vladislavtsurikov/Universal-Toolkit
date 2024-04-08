using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Attributes;
using VladislavTsurikov.SceneUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.Utility.Runtime.GameObjectUtility;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components
{
    [ComponentStack.Runtime.Attributes.MenuItem("Fade Transition")]
    [SceneCollectionComponent]
    public class FadeTransition : SettingsComponent
    {
        public SceneReference SceneReference = new SceneReference();

#if UNITY_EDITOR
        protected override void OnCreate()
        {
            SceneAsset sceneAsset = Resources.Load<SceneAsset>("Fade");

            if (sceneAsset != null)
            {
                SceneReference = new SceneReference(sceneAsset);
            }
        }
#endif
        
        private IEnumerator LoadFadeIfNecessary()
        {
            yield return SceneReference.LoadScene();
            
            SceneOperation sceneOperation = (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];
            
            yield return sceneOperation.OnLoad();
        }

        private IEnumerator UnloadFadeIfNecessary()
        {
            SceneOperation sceneOperation = (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];
            
            yield return sceneOperation.OnUnload();
            
            yield return SceneReference.UnloadScene();
        }

        public override List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>{SceneReference};
        }
        
        internal static IEnumerator LoadFadeIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            FadeTransition fadeTransition = (FadeTransition)settingsList.GetElement(typeof(FadeTransition));

            if (fadeTransition != null)
            {
                yield return fadeTransition.LoadFadeIfNecessary();
            }
        }
        
        internal static IEnumerator UnloadFadeIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponent> settingsList)
        {
            FadeTransition fadeTransition = (FadeTransition)settingsList.GetElement(typeof(FadeTransition));

            if (fadeTransition != null)
            {
                yield return fadeTransition.UnloadFadeIfNecessary();
            }
        }
    }
}