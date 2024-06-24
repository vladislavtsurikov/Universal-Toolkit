using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneUtility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

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
    
    public abstract class SceneType : ComponentStack.Runtime.Core.Component
    {
        private SceneCollection _loadSceneCollection;
        
        public ComponentStackOnlyDifferentTypes<SettingsComponent> SettingsStack = new ComponentStackOnlyDifferentTypes<SettingsComponent>();

        protected override void SetupElement(object[] args = null)
        {
            _loadSceneCollection = (SceneCollection)args[0];

            SettingsStack.Setup();
        }

        internal IEnumerator LoadInternal(bool force = false)
        {
            SceneBehavior sceneBehavior = (SceneBehavior)SettingsStack.GetElement(typeof(SceneBehavior));

            if (!force && sceneBehavior is { SceneOpenBehavior: SceneOpenBehavior.DoNotOpen })
            {
                yield break;
            }
            
            BeforeLoadOperationsSettings beforeLoadOperationsSettings = (BeforeLoadOperationsSettings)SettingsStack.GetElement(typeof(BeforeLoadOperationsSettings));

            if (beforeLoadOperationsSettings != null)
            {
                yield return beforeLoadOperationsSettings.DoOperations();
            }

            yield return Load();
            
            AfterLoadOperationsSettings afterLoadOperationsSettings = (AfterLoadOperationsSettings)SettingsStack.GetElement(typeof(AfterLoadOperationsSettings));

            if (afterLoadOperationsSettings != null)
            {
                yield return afterLoadOperationsSettings.DoOperations();
            }
        }

        internal IEnumerator UnloadInternal(SceneCollection nextLoadSceneCollection = null, bool force = false)
        {
            SceneBehavior sceneBehavior = (SceneBehavior)SettingsStack.GetElement(typeof(SceneBehavior));

            if (!force && sceneBehavior is { SceneCloseBehavior: SceneCloseBehavior.KeepOpenAlways })
            {
                yield break;
            }
            
            BeforeUnloadOperationsSettings beforeUnloadOperationsSettings = (BeforeUnloadOperationsSettings)SettingsStack.GetElement(typeof(BeforeUnloadOperationsSettings));

            if (beforeUnloadOperationsSettings != null)
            {
                yield return beforeUnloadOperationsSettings.DoOperations();
            }

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
            List<SceneReference> sceneReferences = new List<SceneReference>();
            
            foreach (var component in SettingsStack.ElementList)
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