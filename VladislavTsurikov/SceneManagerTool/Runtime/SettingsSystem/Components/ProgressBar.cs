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
    [ComponentStack.Runtime.Attributes.MenuItem("Progress Bar")]
    [SceneCollection]
    public class ProgressBar : SettingsComponentElement
    {
        public SceneReference SceneReference = new SceneReference();
        public bool DisableFade = false;

#if UNITY_EDITOR
        protected override void OnCreate()
        {
            SceneAsset sceneAsset = Resources.Load<SceneAsset>("Progress Bar");

            if (sceneAsset != null)
            {
                SceneReference = new SceneReference(sceneAsset);
            }
        }
#endif

        private IEnumerator LoadFade()
        {
            yield return SceneReference.LoadScene();
            
            SceneOperation sceneOperation = (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];
            
            yield return sceneOperation.OnLoad();
        }

        private IEnumerator UnloadFade()
        {
            SceneOperation sceneOperation = (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];
            
            yield return sceneOperation.OnUnload();
            
            yield return SceneReference.UnloadScene();
        }

        public override List<SceneReference> GetSceneReferences()
        {
            return new List<SceneReference>{SceneReference};
        }
        
        internal static IEnumerator LoadProgressBarIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponentElement> settingsList)
        {
            ProgressBar progressBar = (ProgressBar)settingsList.GetElement(typeof(ProgressBar));
            
            if (progressBar != null)
            {
                yield return progressBar.LoadFade();
            }
        }
        
        internal static IEnumerator UnloadProgressBarIfNecessary(ComponentStackOnlyDifferentTypes<SettingsComponentElement> settingsList)
        {
            ProgressBar progressBar = (ProgressBar)settingsList.GetElement(typeof(ProgressBar));

            if (progressBar != null)
            {
                yield return progressBar.UnloadFade();
            }
        }
    }
}