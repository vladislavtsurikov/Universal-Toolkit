using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem
{
    public class CameraTemporaryComponent : Component
    {
        protected VirtualCamera VirtualCamera;
        
        protected override void SetupElement(object[] args = null)
        {
            VirtualCamera = (VirtualCamera)args[0];
            
            SetupCameraTemporaryComponent();
        }

        protected virtual void SetupCameraTemporaryComponent(){}
    }
}