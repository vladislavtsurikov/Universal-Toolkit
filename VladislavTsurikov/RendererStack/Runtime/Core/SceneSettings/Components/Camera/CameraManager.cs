using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera
{
    [ComponentStack.Runtime.Attributes.MenuItem("Cameras")]
    public partial class CameraManager : SceneComponent
    {
        public List<VirtualCamera> VirtualCameraList = new List<VirtualCamera>();

        [OnDeserializing]
        private void OnDeserializing()
        {
            VirtualCameraList ??= new List<VirtualCamera>();
        }

        protected override void SetupElement(object[] args = null)
        {
            for (int i = VirtualCameraList.Count - 1; i >= 0; i--)
            {
                if (VirtualCameraList[i].CameraType == CameraType.SceneView || VirtualCameraList[i].Camera == null)
                {
                    RemoveCamera(i);
                }
            }

            foreach (VirtualCamera cam in VirtualCameraList)
            {
                cam.Setup();
            }

#if UNITY_EDITOR 
            StartFindSceneViewCamera();

            if (!Application.isPlaying)
            {
                StartEditorSimulation();
            }
#endif
        }

        protected override void OnCreate()
        {
            FindMainCamera();
        }

        protected override void OnDisableElement()
        {
#if UNITY_EDITOR
            EditorApplication.update -= FindSceneViewCamera;
            
            if (!Application.isPlaying)
            {
                StopEditorSimulation();
            }
#endif

            for (int i = 0; i < VirtualCameraList.Count; i++)
            {
                VirtualCameraList[i].OnDisable();
            }
        }

#if UNITY_EDITOR
        private void StartFindSceneViewCamera() 
        {
            EditorApplication.update -= FindSceneViewCamera;
            EditorApplication.update += FindSceneViewCamera;
        }

        private void FindSceneViewCamera()
        {
            UnityEngine.Camera currentCam = UnityEngine.Camera.current;
            if (currentCam != null && currentCam.name == "SceneCamera")
            {
                VirtualCamera sceneViewCameraData = GetSceneCamera();

                if(sceneViewCameraData == null)
                {
                    VirtualCamera sceneviewCamera =
                        new VirtualCamera(null)
                        {
                            CameraType = CameraType.SceneView,
                        };

                    AddCamera(sceneviewCamera);
                    sceneViewCameraData = sceneviewCamera;
                }

                if (sceneViewCameraData.Camera == null)
                {
                    sceneViewCameraData.Camera = currentCam;
                }
            }
        }
#endif

        public void FindMainCamera()
        {
            UnityEngine.Camera selectedCamera = UnityEngine.Camera.main;

            if (selectedCamera == null)
            {
                UnityEngine.Camera[] cameras = Object.FindObjectsOfType<UnityEngine.Camera>();
                for (int i = 0; i <= cameras.Length - 1; i++)
                {
                    if (cameras[i].gameObject.name.Contains("Main Camera") ||
                        cameras[i].gameObject.name.Contains("MainCamera"))
                    {
                        selectedCamera = cameras[i];
                        break;
                    }
                }
                
            }
            
            AddCamera(selectedCamera);
        }

        public void FindAllCamera()
        {
            UnityEngine.Camera[] cameras = Object.FindObjectsOfType<UnityEngine.Camera>();
            for (int i = 0; i <= cameras.Length - 1; i++)
            {
                AddCamera(cameras[i]);
            }
        }

        public void AddCamera(UnityEngine.Camera camera)
        {
            VirtualCamera virtualCamera = GetVirtualCamera(camera);
            if (virtualCamera == null)
            {
                virtualCamera = new VirtualCamera(camera);

                AddCamera(virtualCamera);
            }
        }

        private void AddCamera(VirtualCamera virtualCamera)
        {
            if(GetVirtualCamera(virtualCamera.Camera) == null) 
            {
                VirtualCameraList.Add(virtualCamera);
            }
        }

        public void RemoveCamera(int index)
        {
            VirtualCamera virtualCamera = VirtualCameraList[index];
            if (virtualCamera != null)
            {
                VirtualCameraList.RemoveAt(index);
                virtualCamera.CameraTemporaryComponentStack?.OnDisable();
            }
        }

        public void RemoveCamera(UnityEngine.Camera camera)
        {
            VirtualCamera virtualCamera = GetVirtualCamera(camera);
            if (virtualCamera != null)
            {
                VirtualCameraList.Remove(virtualCamera);
                virtualCamera.CameraTemporaryComponentStack?.OnDisable();
            }
        }
        
        public VirtualCamera GetVirtualCamera(UnityEngine.Camera unityCamera)
        {
            foreach (VirtualCamera cam in VirtualCameraList)
            {
                if (cam.Camera == unityCamera)
                {
                    return cam;
                }
            }
            return null;
        }

        public VirtualCamera GetSceneCamera()
        {
            foreach (VirtualCamera cam in VirtualCameraList) 
            {
                if (cam.CameraType == CameraType.SceneView)
                {
                    return cam;
                }
            }

            return null;
        }
        
        public bool IsMultipleCameras()
        {
            if (Application.isPlaying)
            {
                return VirtualCameraList.Count > 1;
            }
            else
            {
                return VirtualCameraList.Count > 2;
            }
        }
    }
}