using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    [Serializable]
    public abstract class SceneData : Runtime_Core_Component
    {
        public SceneDataManager SceneDataManager;
        
        public delegate void OnSetup ();
        public static OnSetup OnSetupEvent;
        
        protected override UniTask SetupComponent(object[] setupData = null)
        {
            if (setupData != null)
            {
                SceneDataManager = (SceneDataManager)setupData[0]; 
            }

            SetupSceneData();
            OnSetupEvent?.Invoke();
            return UniTask.CompletedTask;
        }

        protected override void OnCreate()
        {
            if (SceneDataManager == null)
            {
                return;
            }
                
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(SceneDataManager.Scene);
            }
#endif
        }

        protected override void OnDeleteElement()
        {
            if (SceneDataManager == null)
            {
                return;
            }
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(SceneDataManager.Scene);
            }
#endif
        }

        public override bool DeleteElement()
        {
            AllowCreateComponentAttribute allowCreateComponentAttribute = GetType().GetAttribute<AllowCreateComponentAttribute>();

            if (allowCreateComponentAttribute == null)
            {
                return true;
            }

            return allowCreateComponentAttribute.Allow(SceneDataManager);
        }

        protected virtual void SetupSceneData(){}
        public virtual void DrawDebug(){}
        public virtual void LateUpdate(){}
    }
}