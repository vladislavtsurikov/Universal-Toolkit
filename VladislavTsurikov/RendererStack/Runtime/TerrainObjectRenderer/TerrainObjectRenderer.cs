using UnityEngine.SceneManagement;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings.Components;
using VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraSettingsSystem.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraTemporarySettingsSystem.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraTemporarySettingsSystem.Components.ObjectCameraRender;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GlobalSettings.Components.ExtensionSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GPUInstancedIndirect.ComputeShaderKernelProperties;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Components;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Components.Camera;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Components;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Components.Scripting;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.SceneSettings.Components.Camera;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using LODGroup = VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings.Components.LODGroup;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer
{
    [MenuItem("Terrain Object Renderer")]
    [PrototypeType(typeof(PrototypeTerrainObject))]
    [GenerateGPUInstancedIndirectShaders]
    [AddSceneData(new[] { typeof(TerrainObjectRendererData), typeof(TerrainManager)})]
    [AddCameraTemporaryComponents(new[] { typeof(ObjectCameraRender), typeof(CameraCollidersController)})]
    [AddCameraComponents(new[] { typeof(TerrainObjectRendererCameraSettings)})]
    [AddSceneComponents(new[] { typeof(CameraManager), typeof(Quality) })]
    [AddGlobalComponents(new[] { typeof(Common.GlobalSettings.Components.Quality), typeof(ExtensionSystem) })]
    [AddPrototypeComponents(new[] { typeof(LODGroup), typeof(Shadow), typeof(DistanceCulling), typeof(FrustumCulling), typeof(Colliders), typeof(Scripting) })]
    public partial class TerrainObjectRenderer : PrototypeRenderer
    {
        private static TerrainObjectRenderer _sInstance;

        public static TerrainObjectRenderer Instance
        {
            get
            {
                if (RendererStackManager.Instance == null)
                {
                    return null;
                }

                _sInstance = (TerrainObjectRenderer)RendererStackManager.Instance.RendererStack.GetElement(typeof(TerrainObjectRenderer));

                if (_sInstance == null)
                {
                    _sInstance = (TerrainObjectRenderer)RendererStackManager.Instance.RendererStack.CreateIfMissingType(typeof(TerrainObjectRenderer));
                }

                return _sInstance;
            }
        }

        public ScriptingSystem.ScriptingSystem ScriptingSystem;
        public bool DebugAllCells = false;
        public bool DebugVisibleCells = false;
        
        public delegate void SetupPrototypeRendererRendererAfter ();
        public static SetupPrototypeRendererRendererAfter AfterSetupPrototypeRendererRendererEvent;

        protected override void SetupPrototypeRendererRenderer()
        {
            if (ScriptingSystem == null)
            {
                ScriptingSystem = new ScriptingSystem.ScriptingSystem();
            }
            
            MergeInstancedIndirectBuffersID.Setup();
            GPUFrustumCullingID.Setup();

            ScriptingSystem.Setup();

            TerrainManager.ChangedTerrainCountEvent -= NewTerrainAction;
            TerrainManager.ChangedTerrainCountEvent += NewTerrainAction;

            AfterSetupPrototypeRendererRendererEvent?.Invoke(); 
        }

        protected override void OnDisableElement()
        {
            ScriptingSystem?.OnDisable();
            MergeInstancedIndirectBuffersID.Dispose();
            GPUFrustumCullingID.Dispose();
        }

        public void NewTerrainAction(SceneDataManager sceneDataManager)
        {
            if (SceneManager.sceneCount == 1)
            {
                TerrainObjectRendererData.RefreshAllCells();
            }
        }

        public override void DrawDebug()
        {
            foreach (var item in SceneDataStackUtility.GetAllSceneData<TerrainObjectRendererData>())
            {
                item.DrawDebug();
            }
        }
    }
}