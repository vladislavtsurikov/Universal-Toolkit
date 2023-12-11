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
        [SerializeField]
        private string _name;
        
        public int ID => _id;

        public SceneComponentStack SceneComponentStack = new SceneComponentStack();
        
        public override string Name
        {
            get => _name;
            set => _name = value;
        }

        public bool Startup = true;
        
        public static SceneCollection Current { get; private set; }

        public ComponentStackOnlyDifferentTypes<SettingsComponentElement> SettingsList = new ComponentStackOnlyDifferentTypes<SettingsComponentElement>();

        public float LoadingProgress
        {
            get
            {
                var sceneComponents = new List<SceneType>();

                foreach (var sceneComponent in SceneComponentStack.ElementList)
                {
                    SceneBehavior sceneBehavior = (SceneBehavior)sceneComponent.SettingsList.GetElement(typeof(SceneBehavior));

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
            SceneComponentStack.Setup(true, this);
            SettingsList.Setup();
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
                yield return FadeTransition.LoadFadeIfNecessary(Current.SettingsList);
                yield return ActiveScene.UnloadActiveSceneIfNecessary(Current.SettingsList);
                yield return Current.Unload(this);
            }
            
            if (this != Current)
            {
                Current = this;
                
                yield return ProgressBar.LoadProgressBarIfNecessary(SettingsList);
                
                BeforeLoadOperationsSettings beforeLoadOperationsSettings = (BeforeLoadOperationsSettings)SettingsList.GetElement(typeof(BeforeLoadOperationsSettings));

                if (beforeLoadOperationsSettings != null)
                {
                    yield return beforeLoadOperationsSettings.DoOperations(this);
                }
                
                yield return ActiveScene.LoadActiveSceneIfNecessary(SettingsList);

                foreach (var sceneComponent in SceneComponentStack.ElementList)
                {
                    yield return sceneComponent.LoadInternal();
                }

                while (LoadingProgress != 1)
                {
                    yield return null;
                }
            
                AfterLoadOperationsSettings afterLoadOperationsSettings = (AfterLoadOperationsSettings)SettingsList.GetElement(typeof(AfterLoadOperationsSettings));

                if (afterLoadOperationsSettings != null)
                {
                    yield return afterLoadOperationsSettings.DoOperations(this);
                }
                
                yield return ProgressBar.UnloadProgressBarIfNecessary(SettingsList);

                if (pastSceneCollection != null)
                {
                    yield return FadeTransition.UnloadFadeIfNecessary(pastSceneCollection.SettingsList);
                }
            }
        }

        public IEnumerator Unload(SceneCollection nextLoadSceneCollection)
        {
            Current = null;
            
            BeforeUnloadOperationsSettings beforeUnloadOperationsSettings = (BeforeUnloadOperationsSettings)SettingsList.GetElement(typeof(BeforeUnloadOperationsSettings));

            if (beforeUnloadOperationsSettings != null)
            {
                yield return beforeUnloadOperationsSettings.DoOperations(nextLoadSceneCollection);
            }

            foreach (var sceneComponent in SceneComponentStack.ElementList)
            {
                yield return sceneComponent.UnloadInternal(nextLoadSceneCollection);
            }
            
            AfterUnloadOperationsSettings afterUnloadOperationsSettings = (AfterUnloadOperationsSettings)SettingsList.GetElement(typeof(AfterUnloadOperationsSettings));

            if (afterUnloadOperationsSettings != null)
            {
                yield return afterUnloadOperationsSettings.DoOperations(nextLoadSceneCollection);
            }
        }

        public bool HasScene(SceneReference sceneReference)
        {
            return SceneComponentStack.HasScene(sceneReference);
        }
        
        public List<SceneReference> GetSceneReferences()
        {
            List<SceneReference> sceneReferences = new List<SceneReference>();

            foreach (var sceneComponent in SceneComponentStack.ElementList)
            {
                foreach (var sceneManagerComponent in SettingsList.ElementList)
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