using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
#if UNITY_EDITOR
using VladislavTsurikov.GameObjectCollider.Editor;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public enum HandleSettingsMode
    { 
        Custom,
        Standard
    }
    
    [Name("Area Settings")]
    public class Area : Runtime_Core_Component
    {
        public Vector3 PastThisPosition = Vector3.zero;
        public Vector3 PastScale = Vector3.one;
        public Bounds Bounds;

        public StamperTool StamperTool;

        public Action OnSetAreaBounds;
        
        public Color ColorCube = Color.HSVToRGB(0.0f, 0.75f, 1.0f);
        public float PixelWidth = 4.0f;
        public bool Dotted;
        public HandleSettingsMode HandleSettingsMode = HandleSettingsMode.Standard;
        public bool DrawHandleIfNotSelected;

        protected override UniTask SetupComponent(object[] setupData = null)
        {
            StamperTool = (StamperTool)setupData[0];
            SetupArea();
            return UniTask.CompletedTask;
        }

        protected virtual void SetupArea()
        {
        }
        
        public virtual Texture2D GetCurrentRaw()
        {
            return Texture2D.whiteTexture;
        }

        public void SetAreaBounds(StamperTool stamperToolTool)
        {
            var transform = stamperToolTool.transform;
            var localScale = transform.localScale;
            Bounds = new Bounds
            {
                size = new Vector3(localScale.x, localScale.y, localScale.z),
                center = transform.position
            };
        }

#if UNITY_EDITOR
        public void FitToWorldSize()
        {
            GameObjectCollider.Editor.GameObjectCollider gameObjectCollider = SceneDataStackUtility.InstanceSceneData<GameObjectCollider.Editor.GameObjectCollider>(SceneManager.GetActiveScene());
            gameObjectCollider.RefreshObjectTree();

            AABB sceneAABB = gameObjectCollider.GetAABB();
            
            StamperTool.transform.localScale = sceneAABB.Size;
            StamperTool.transform.position = sceneAABB.Center;
        }
#endif

        public void SetAreaBoundsIfNecessary(StamperTool stamperToolTool, bool setForce = false)
        {
            bool hasChangedPosition = PastThisPosition != stamperToolTool.transform.position;
            bool hasChangedSize = stamperToolTool.transform.localScale != PastScale;

            if(hasChangedPosition || hasChangedSize)
            {
                OnSetAreaBounds?.Invoke();

                SetAreaBounds(stamperToolTool);
            }
            else if(setForce)
            {
                OnSetAreaBounds?.Invoke();

                SetAreaBounds(stamperToolTool);
            }

            PastScale = stamperToolTool.transform.localScale;

            PastThisPosition = stamperToolTool.transform.position;
        }

        public BoxArea GetAreaVariables(RayHit hit)
        {
            BoxArea area = new BoxArea(hit, Bounds.size.x)
            {
                Mask = GetCurrentRaw(),
            };

            return area;
        }
    }
}