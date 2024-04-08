using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components;
using VladislavTsurikov.SceneUtility.Runtime;
using ProgressBar = VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components.ProgressBar;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem
{
    public class SceneCollection : ComponentStack.Runtime.Component
    {
        [OdinSerialize] 
        private int _id;
        [OdinSerialize] 
        private string _name;
        
        public int ID => _id;
        
        [OdinSerialize]
        public SceneTypeComponentStack SceneTypeComponentStack = new SceneTypeComponentStack();
        
        public override string Name
        {
            get => _name;
            set => _name = value;
        }

        public bool Startup = true;
        
        public static SceneCollection Current { get; private set; }
        
        [OdinSerialize]
        public ComponentStackOnlyDifferentTypes<SettingsComponent> SettingsStack = new ComponentStackOnlyDifferentTypes<SettingsComponent>();

        public float LoadingProgress
        {
            get
            {
                var sceneComponents = new List<SceneType>();

                foreach (var sceneComponent in SceneTypeComponentStack.ElementList)
                {
                    SceneBehavior sceneBehavior = (SceneBehavior)sceneComponent.SettingsStack.GetElement(typeof(SceneBehavior));

                    if (sceneBehavior != null)
                    {
                        if (sceneBehavior.SceneOpenBehavior == SceneOpenBehavior.DoNotOpen)
                        {
                            continue;
                        }
                    }
                    
                    sceneComponents.Add(sceneComponent);
                }
                
                return !sceneComponents.Any() ? 1 : sceneComponents.Sum(a => a.LoadingProgress()) / sceneComponents.Count;
            }
        }
        
        protected override void SetupElement(object[] args = null)
        {
            SceneTypeComponentStack.Setup(true, this);
            SettingsStack.Setup();
        }

        protected override void OnCreate()
        {
            _id = GetHashCode();
            _name = "Scene Collection";
        }

        public IEnumerator Load()
        {
            SceneCollection pastSceneCollection = Current;
            
            if (Current != null)
            {
                yield return FadeTransition.LoadFadeIfNecessary(Current.SettingsStack);
                yield return ActiveScene.UnloadActiveSceneIfNecessary(Current.SettingsStack);
                yield return Current.Unload(this);
            }

            if (this == Current)
            {
                yield break;
            }
            
            Current = this;
                
            yield return ProgressBar.LoadProgressBarIfNecessary(SettingsStack);
                
            BeforeLoadOperationsSettings beforeLoadOperationsSettings = (BeforeLoadOperationsSettings)SettingsStack.GetElement(typeof(BeforeLoadOperationsSettings));

            if (beforeLoadOperationsSettings != null)
            {
                yield return beforeLoadOperationsSettings.DoOperations();
            }
                
            yield return ActiveScene.LoadActiveSceneIfNecessary(SettingsStack);

            foreach (var sceneComponent in SceneTypeComponentStack.ElementList)
            {
                yield return sceneComponent.LoadInternal();
            }

            while (LoadingProgress != 1)
            {
                yield return null;
            }
            
            AfterLoadOperationsSettings afterLoadOperationsSettings = (AfterLoadOperationsSettings)SettingsStack.GetElement(typeof(AfterLoadOperationsSettings));

            if (afterLoadOperationsSettings != null)
            {
                yield return afterLoadOperationsSettings.DoOperations();
            }
                
            yield return ProgressBar.UnloadProgressBarIfNecessary(SettingsStack);

            if (pastSceneCollection != null)
            {
                yield return FadeTransition.UnloadFadeIfNecessary(pastSceneCollection.SettingsStack);
            }
        }

        public IEnumerator Unload(SceneCollection nextLoadSceneCollection)
        {
            Current = null;
            
            BeforeUnloadOperationsSettings beforeUnloadOperationsSettings = (BeforeUnloadOperationsSettings)SettingsStack.GetElement(typeof(BeforeUnloadOperationsSettings));

            if (beforeUnloadOperationsSettings != null)
            {
                yield return beforeUnloadOperationsSettings.DoOperations();
            }

            foreach (var sceneComponent in SceneTypeComponentStack.ElementList)
            {
                yield return sceneComponent.UnloadInternal(nextLoadSceneCollection);
            }
            
            AfterUnloadOperationsSettings afterUnloadOperationsSettings = (AfterUnloadOperationsSettings)SettingsStack.GetElement(typeof(AfterUnloadOperationsSettings));

            if (afterUnloadOperationsSettings != null)
            {
                yield return afterUnloadOperationsSettings.DoOperations();
            }
        }

        public bool HasScene(SceneReference sceneReference)
        {
            return SceneTypeComponentStack.HasScene(sceneReference);
        }
        
        public List<SceneReference> GetSceneReferences()
        {
            List<SceneReference> sceneReferences = new List<SceneReference>();

            foreach (var sceneComponent in SceneTypeComponentStack.ElementList)
            {
                foreach (var sceneManagerComponent in SettingsStack.ElementList)
                {
                    sceneReferences.AddRange(sceneManagerComponent.GetSceneReferences());
                }
                
                sceneReferences.AddRange(sceneComponent.GetSceneReferencesInternal());
            }
            
            return sceneReferences;
        }
        
        public bool IsLoaded() => Current == this;
    }
}