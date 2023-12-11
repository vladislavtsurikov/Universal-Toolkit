using System;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraTemporarySettingsSystem;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.SceneSettings.Components.Camera
{
    public class CameraCollidersController : CameraTemporaryComponent
    {
        [NonSerialized]
        public bool UsedForColliders = false;

        protected override void OnDisable()
        {
            ScriptingSystem.RemoveColliders(VirtualCamera);
        }
    }
}