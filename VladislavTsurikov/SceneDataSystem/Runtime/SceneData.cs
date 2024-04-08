using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Attributes;

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    [Serializable]
    public abstract class SceneData : ComponentStack.Runtime.Component
    {
        public SceneDataManager SceneDataManager;
        
        public delegate void OnSetup ();
        public static OnSetup OnSetupEvent;
        
        protected override void SetupElement(object[] args = null)
        {
            if (args != null)
            {
                SceneDataManager = (SceneDataManager)args[0]; 
            }

            SetupSceneData();
            OnSetupEvent?.Invoke();
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

        public override bool IsValid()
        {
            AllowCreateComponentAttribute allowCreateComponentAttribute = GetType().GetAttribute<AllowCreateComponentAttribute>();
                
            if(allowCreateComponentAttribute == null)
                return true;

            return allowCreateComponentAttribute.Allow(SceneDataManager);
        }

        protected virtual void SetupSceneData(){}
        public virtual void DrawDebug(){}
        public virtual void LateUpdate(){}
    }
}