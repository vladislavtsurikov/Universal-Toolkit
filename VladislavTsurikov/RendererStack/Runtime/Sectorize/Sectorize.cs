using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    [Name("Sectorize - Terrain Streaming")]
    [AddSceneData(new []{typeof(SectorLayerManager)})]
    [AddSceneComponents(new[]{typeof(CameraManager)})]
    [AddGlobalComponents(new[]{typeof(StreamingRules)})]
    public partial class Sectorize : Renderer
    {
        private static Sectorize s_instance;

        public static Sectorize Instance
        {
            get
            {
                s_instance = (Sectorize)RendererStackManager.Instance.RendererStack.GetElement(typeof(Sectorize));

                if (s_instance == null)
                {
                    s_instance = (Sectorize)RendererStackManager.Instance.RendererStack.CreateIfMissingType(typeof(Sectorize));
                }

                return s_instance;
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
                StreamingUtility.LoadAllScenes(GetSectorLayerTag()).Forget();
            }
        }
    }
}