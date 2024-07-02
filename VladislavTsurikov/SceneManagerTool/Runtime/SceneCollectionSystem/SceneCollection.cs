using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneUtility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using ProgressBar = VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.ProgressBar;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem
{
    public class SceneCollection : Component
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
        
        protected override void SetupComponent(object[] setupData = null)
        {
            SceneTypeComponentStack.Setup(true, this);
            SettingsStack.Setup();
        }

        protected override void OnCreate()
        {
            _id = GetHashCode();
            _name = "Scene Collection";
        }

        public async UniTask Load()
        {
            SceneCollection pastSceneCollection = Current;
            
            if (Current != null)
            {
                await FadeTransition.LoadFadeIfNecessary(Current.SettingsStack);
                await Current.Unload(this);
            }

            if (this == Current)
            {
                return;
            }
            
            Current = this;
                
            await ProgressBar.LoadProgressBarIfNecessary(SettingsStack);
                
            BeforeLoadOperationsSettings beforeLoadOperationsSettings = (BeforeLoadOperationsSettings)SettingsStack.GetElement(typeof(BeforeLoadOperationsSettings));

            if (beforeLoadOperationsSettings != null)
            {
                await beforeLoadOperationsSettings.DoOperations();
            }
                
            await ActiveScene.LoadActiveSceneIfNecessary(SettingsStack);

            foreach (var sceneComponent in SceneTypeComponentStack.ElementList)
            {
                await sceneComponent.LoadInternal();
            }
            
            await UniTask.WaitWhile(() => LoadingProgress != 1);
            
            AfterLoadOperationsSettings afterLoadOperationsSettings = (AfterLoadOperationsSettings)SettingsStack.GetElement(typeof(AfterLoadOperationsSettings));

            if (afterLoadOperationsSettings != null)
            {
                await afterLoadOperationsSettings.DoOperations();
            }

            await ProgressBar.UnloadProgressBarIfNecessary(SettingsStack);

            if (pastSceneCollection != null)
            {
                await FadeTransition.UnloadFadeIfNecessary(pastSceneCollection.SettingsStack);
            }
        }

        public async UniTask Unload(SceneCollection nextLoadSceneCollection)
        {
            Current = null;
            
            BeforeUnloadOperationsSettings beforeUnloadOperationsSettings = (BeforeUnloadOperationsSettings)SettingsStack.GetElement(typeof(BeforeUnloadOperationsSettings));

            if (beforeUnloadOperationsSettings != null)
            {
                await beforeUnloadOperationsSettings.DoOperations();
            }

            foreach (var sceneComponent in SceneTypeComponentStack.ElementList)
            {
                await sceneComponent.UnloadInternal(nextLoadSceneCollection);
            }
            
            await ActiveScene.UnloadActiveSceneIfNecessary(SettingsStack);
            
            AfterUnloadOperationsSettings afterUnloadOperationsSettings = (AfterUnloadOperationsSettings)SettingsStack.GetElement(typeof(AfterUnloadOperationsSettings));

            if (afterUnloadOperationsSettings != null)
            {
                await afterUnloadOperationsSettings.DoOperations();
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