using System;
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings.Components;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraSettingsSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraSettingsSystem.Attributes;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraTemporarySettingsSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera.CameraTemporarySettingsSystem.Attributes;
#if UNITY_EDITOR 
using VladislavTsurikov.RendererStack.Editor.Core.SceneSettings.Components.Camera.CameraSettingsSystem;
#endif

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Components.Camera
{
    public enum CameraType
    {
        Normal,
        SceneView
    }

    [Serializable]
    public class VirtualCamera
    {
        [NonSerialized] 
        private float _lastMaxDistance;
        [NonSerialized] 
        private Matrix4x4 _lastCameraTransform;
        [NonSerialized] 
        private float _lastFieldOfView;

        public UnityEngine.Camera Camera;
        public bool Active = true;
        
        public CameraType CameraType = CameraType.Normal;
        public Vector3 FloatingOriginOffset = new Vector3(0,0,0);

        public bool FoldoutGUI = true; 
        public bool Ignored => !IsCameraActive();
        
        [NonSerialized]
        public ComponentStackOnlyDifferentTypes<CameraTemporaryComponent> CameraTemporaryComponentStack;
        
        [OdinSerialize]
        public ComponentStackOnlyDifferentTypes<CameraComponent> CameraComponentStack = new ComponentStackOnlyDifferentTypes<CameraComponent>();
        
#if UNITY_EDITOR
        public CameraComponentStackEditor CameraComponentStackEditor;
#endif
        
        [OnDeserializing]
        private void OnDeserializing()
        {
            CameraComponentStack ??= new ComponentStackOnlyDifferentTypes<CameraComponent>();
        }
        
        public void Setup()
        {
            OnDisable();
            CameraTemporaryComponentStack = new ComponentStackOnlyDifferentTypes<CameraTemporaryComponent>();
            CameraTemporaryComponentStack.Setup(true, this);
            
            CameraComponentStack.Setup();
            
            foreach (var renderer in RendererStackManager.Instance.RendererStack.ElementList)
            {
                AddCameraTemporaryComponentsAttribute addCameraTemporaryComponentsAttribute = (AddCameraTemporaryComponentsAttribute)renderer.GetType().GetAttribute(typeof(AddCameraTemporaryComponentsAttribute));
                AddCameraComponentsAttribute addCameraComponentsAttribute = (AddCameraComponentsAttribute)renderer.GetType().GetAttribute(typeof(AddCameraComponentsAttribute));

                if (addCameraTemporaryComponentsAttribute != null)
                {
                    CameraTemporaryComponentStack.CreateIfMissingType(addCameraTemporaryComponentsAttribute.Types);
                }

                if (addCameraComponentsAttribute != null)
                {
                    CameraComponentStack.CreateIfMissingType(addCameraComponentsAttribute.Types);
                }
            }
            
#if UNITY_EDITOR
            CameraComponentStackEditor = new CameraComponentStackEditor(CameraComponentStack);
#endif    
        }

        public VirtualCamera(UnityEngine.Camera selectedCamera)
        {
            Camera = selectedCamera; 
            Setup();
        }

        public UnityEngine.Camera GetRenderingCamera()
        {
#if UNITY_EDITOR
            if(Application.isPlaying == false)
            {
                if(RendererStackManager.Instance.EditorPlayModeSimulation)
                {
                    return null;
                }
            }
#endif
            if(RendererStackSettings.Instance.RenderDirectToCamera)
            {
                if (CameraType == CameraType.SceneView || Application.isPlaying)
                {
                    return Camera;
                }
            }
            else
            {
                if (CameraType == CameraType.SceneView)
                {
                    return Camera;
                }
            }
                
            return null;
        }
        
        public bool IsCameraActive()
        {
#if UNITY_EDITOR
            bool fitCamera = IsFitCamera();

            if (!fitCamera)
            {
                return false;
            }
            
            if (Application.isPlaying == false)
            {
                if(RendererStackManager.Instance.EditorPlayModeSimulation)
                {
                    if(CameraType == CameraType.SceneView)
                    {
                        return false;
                    } 
                }
                else
                {
                    if(CameraType == CameraType.Normal)
                    {
                        return false;
                    } 
                }

                return true;
            }
            else
            {
                if(CameraType == CameraType.SceneView)
                {
                    if(RendererStackSettings.Instance.RenderSceneCameraInPlayMode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            
                return true;
            }
#else
            return IsFitCamera();
#endif
        }

        public bool IsFitCamera()
        {
            if(CameraType == CameraType.Normal)
            {
                if(Camera == null || Camera.gameObject.activeInHierarchy == false || Camera.enabled == false)
                {
                    return false;
                }

                return Active;
            }
            
            return Camera != null;
        }

        public Vector3 GetCameraPosition()
        {
            return Camera.transform.position - FloatingOriginOffset;
        }

        public float GetMaxDistance(Type rendererType)
        {
            Quality quality = (Quality)GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality), rendererType);

            float maxDistance = Mathf.Min(Camera.farClipPlane, quality.MaxRenderDistance);

            float fieldOfViewFactor = 90 / Mathf.Clamp(Camera.fieldOfView, 0, 90);

            maxDistance *= fieldOfViewFactor;
            
            return maxDistance;
        }

        public Matrix4x4 GetMatrix4X4()
        {
            var transform = Camera.transform;
            return Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        }

        public bool IsPositionChanged()
        {
            bool needUpdate = !_lastCameraTransform.GetPosition().Equals(Camera.transform.position);
            
            _lastCameraTransform = GetMatrix4X4();

            return needUpdate;
        }

        public bool IsNeedUpdateRenderData(Type rendererType)
        {
            bool needUpdate = _lastMaxDistance != GetMaxDistance(rendererType) || !_lastCameraTransform.Equals(GetMatrix4X4()) || _lastFieldOfView != Camera.fieldOfView;
            
            return needUpdate;
        }

        public void RefreshCameraData(Type rendererType)
        {
            _lastMaxDistance = GetMaxDistance(rendererType);
            _lastCameraTransform = GetMatrix4X4();
            _lastFieldOfView = Camera.fieldOfView;
        }

        public void OnDisable()
        {
            CameraTemporaryComponentStack?.OnDisable();
        }
    }
}