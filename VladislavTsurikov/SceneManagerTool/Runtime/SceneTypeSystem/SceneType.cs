using System.Collections;
using System.Collections.Generic;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem
{
    public enum SceneCloseBehavior
    {
        Close,
        KeepOpenAlways
    }

    public enum SceneOpenBehavior
    {
        Open,
        DoNotOpen
    }
    
    public abstract class SceneType : Component
    {
        private SceneCollection _loadSceneCollection;
        
        public ComponentStackOnlyDifferentTypes<SettingsComponentElement> SettingsList = new ComponentStackOnlyDifferentTypes<SettingsComponentElement>();

        protected override void SetupElement(object[] args = null)
        {
            _loadSceneCollection = (SceneCollection)args[0];

            SettingsList.Setup();
        }

        internal IEnumerator LoadInternal(bool force = false)
        {
            SceneBehavior sceneBehavior = (SceneBehavior)SettingsList.GetElement(typeof(SceneBehavior));

            if (!force && sceneBehavior != null)
            {
                if (sceneBehavior.SceneOpenBehavior == SceneOpenBehavior.DoNotOpen)
                    yield break;
            }
            
            BeforeLoadOperationsSettings beforeLoadOperationsSettings = (BeforeLoadOperationsSettings)SettingsList.GetElement(typeof(BeforeLoadOperationsSettings));
            
            if(beforeLoadOperationsSettings != null)
                yield return beforeLoadOperationsSettings.DoOperations(_loadSceneCollection);

            yield return Load();
            
            AfterLoadOperationsSettings afterLoadOperationsSettings = (AfterLoadOperationsSettings)SettingsList.GetElement(typeof(AfterLoadOperationsSettings));
            
            if(afterLoadOperationsSettings != null)
                yield return afterLoadOperationsSettings.DoOperations(_loadSceneCollection);
        }

        internal IEnumerator UnloadInternal(SceneCollection nextLoadSceneCollection = null, bool force = false)
        {
            SceneBehavior sceneBehavior = (SceneBehavior)SettingsList.GetElement(typeof(SceneBehavior));

            if (!force && sceneBehavior != null)
            {
                if(sceneBehavior.SceneCloseBehavior == SceneCloseBehavior.KeepOpenAlways)
                    yield break;
            }
            
            BeforeUnloadOperationsSettings beforeUnloadOperationsSettings = (BeforeUnloadOperationsSettings)SettingsList.GetElement(typeof(BeforeUnloadOperationsSettings));
            
            if(beforeUnloadOperationsSettings != null)
                yield return beforeUnloadOperationsSettings.DoOperations(nextLoadSceneCollection);

            yield return Unload(nextLoadSceneCollection);
        }
        
        internal IEnumerator UnloadSceneReference(SceneCollection nextLoadSceneCollection, SceneReference scene)
        {
            if (nextLoadSceneCollection == null)
            {
                yield return scene.UnloadScene();
            }
            else if (!nextLoadSceneCollection.HasScene(scene))
            {
                yield return scene.UnloadScene();
            }
        }

        internal List<SceneReference> GetSceneReferencesInternal()
        {
            List<SceneReference> sceneReferences =
                new List<SceneReference>();
            
            foreach (var component in SettingsList.ElementList)
            {
                sceneReferences.AddRange(component.GetSceneReferences());
            }
            
            sceneReferences.AddRange(GetSceneReferences());
            
            return sceneReferences;
        }

        protected abstract IEnumerator Load();
        protected abstract IEnumerator Unload(SceneCollection nextLoadSceneCollection);
        public abstract bool HasScene(SceneReference sceneReference);
        protected abstract List<SceneReference> GetSceneReferences();
        public abstract float LoadingProgress();
    }
}