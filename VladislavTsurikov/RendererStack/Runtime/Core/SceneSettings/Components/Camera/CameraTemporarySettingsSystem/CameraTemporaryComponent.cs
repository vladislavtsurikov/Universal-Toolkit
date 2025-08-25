using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem
{
    public class CameraTemporaryComponent : Component
    {
        protected VirtualCamera VirtualCamera;

        protected override UniTask SetupComponent(object[] setupData = null)
        {
            VirtualCamera = (VirtualCamera)setupData[0];

            SetupCameraTemporaryComponent();

            return UniTask.CompletedTask;
        }

        protected virtual void SetupCameraTemporaryComponent()
        {
        }
    }
}
