using System;
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Editor.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Editor.Core.SceneSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Attributes;

namespace VladislavTsurikov.RendererStack.Runtime.Core
{
    [SceneType(SceneType.ParentScene)]
    public class RendererStackManager : SingletonSceneData<RendererStackManager>
    {
        [NonSerialized]
        private bool _editorPlayModeSimulation;

        public bool EditorPlayModeSimulation
        {
            get
            {
                if (Application.isPlaying)
                {
                    return false;
                }
                
                return _editorPlayModeSimulation;
            }
            set
            {
                if (_editorPlayModeSimulation != value)
                {
                    _editorPlayModeSimulation = value;
                    OnEditorPlayModeSimulationEvent?.Invoke(_editorPlayModeSimulation);
                }
            }
        }
        
        [OdinSerialize]
        public RendererSystem.RendererStack RendererStack = new RendererSystem.RendererStack();
        [OdinSerialize]
        public SceneComponentStack SceneComponentStack = new SceneComponentStack(); 
        
        public delegate void OnEditorPlayModeSimulation (bool editorPlayModeSimulation);
        public static OnEditorPlayModeSimulation OnEditorPlayModeSimulationEvent;

#if UNITY_EDITOR 
        [NonSerialized]
        public RendererStackEditor RendererStackStackEditor;
        [NonSerialized]
		public SceneComponentStackEditor SceneComponentStackEditor;
#endif

        [OnDeserializing]
        private void OnDeserializing()
        {
            SceneComponentStack ??= new SceneComponentStack();
            RendererStack ??= new RendererSystem.RendererStack();
        }
        
        public override void LateUpdate()
        {
            if(!IsSetup)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                RendererStack.CheckChanges();  
            }
#endif
            
            RendererStack.Render();
        }
        
        protected override void SetupSceneData()
        {
            RendererStack.ForceUpdateRendererData();
#if UNITY_EDITOR
            SceneComponentStackEditor ??= new SceneComponentStackEditor(SceneComponentStack);
            RendererStackStackEditor ??= new RendererStackEditor(RendererStack);
#endif
            
            RendererStack.Setup();

            SceneComponentStack.CreateIfMissingType(typeof(CameraManager));
            SceneComponentStack.Setup();

            GlobalSettings.GlobalSettings.Instance.Setup();
        }

        protected override void OnDisable()
        {
            SceneComponentStack.OnDisable(); 
            RendererStack.OnDisable();
        }

#if UNITY_EDITOR
        public override void DrawDebug()
        {
            foreach (var customRenderer in RendererStack.ElementList)
            {
                SceneComponentStack.DrawDebug(customRenderer);
                GlobalSettings.GlobalSettings.Instance.DrawGizmos(customRenderer);

                customRenderer.DrawDebug();
            }
        }
#endif
    }
}