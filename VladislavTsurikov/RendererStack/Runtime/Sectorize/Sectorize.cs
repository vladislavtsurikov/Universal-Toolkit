using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Coroutines.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility.Utility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    [MenuItem("Sectorize - Terrain Streaming")]
    [AddSceneData(new []{typeof(SectorLayerManager)})]
    [AddSceneComponents(new[]{typeof(CameraManager)})]
    [AddGlobalComponents(new[]{typeof(StreamingRules)})]
    public partial class Sectorize : CustomRenderer
    {
        private static Sectorize _sInstance;

        public static Sectorize Instance
        {
            get
            {
                _sInstance = (Sectorize)RendererStackManager.Instance.RendererStack.GetElement(typeof(Sectorize));

                if (_sInstance == null)
                {
                    _sInstance = (Sectorize)RendererStackManager.Instance.RendererStack.CreateIfMissingType(typeof(Sectorize));
                }

                return _sInstance;
            }
        }

        public bool DebugAllCells = false;
        public bool DebugVisibleCells = false;
        
        public delegate void CreateScenesAfter ();
        public static CreateScenesAfter CreateScenesAfterEvent;
        
        public static string GetSectorLayerTag()
        {
            return "Terrain";
        }
        
        protected override void SetupRenderer()
        {
            RendererStackManager.OnEditorPlayModeSimulationEvent -= OnEditorPlayModeSimulationEvent;
            RendererStackManager.OnEditorPlayModeSimulationEvent += OnEditorPlayModeSimulationEvent;
            
#if UNITY_EDITOR
            Editor.Sectorize.Integration.SceneManagerIntegration.PrepareSceneManager();
#endif
        }

        private void OnEditorPlayModeSimulationEvent(bool editorPlayModeSimulation)
        {
            if (editorPlayModeSimulation == false)
            {
                CoroutineRunner.StartCoroutine(StreamingUtility.LoadAllScenes(GetSectorLayerTag()));
            }
        }
    }
}