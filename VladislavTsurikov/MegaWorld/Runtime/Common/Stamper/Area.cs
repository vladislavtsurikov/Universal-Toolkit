using System;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public enum HandleSettingsMode
    { 
        Custom,
        Standard
    }

    [Name("Area Settings")]
    public class Area : Component
    {
        public Vector3 PastThisPosition = Vector3.zero;
        public Vector3 PastScale = Vector3.one;
        public Bounds Bounds;

        public Color ColorCube = Color.HSVToRGB(0.0f, 0.75f, 1.0f);
        public float PixelWidth = 4.0f;
        public bool Dotted;
        public HandleSettingsMode HandleSettingsMode = HandleSettingsMode.Standard;
        public bool DrawHandleIfNotSelected;

        public StamperTool StamperTool;

        public Action OnSetAreaBounds;

        protected override void SetupComponent(object[] setupData = null)
        {
            StamperTool = (StamperTool)setupData[0];
            SetupArea();
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

        public void FitToTerrainSize(StamperTool stamperToolTool)
        {
            if(Terrain.activeTerrains.Length != 0)
            {
                Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);
                for (int i = 0; i < Terrain.activeTerrains.Length; i++)
                {
                    Terrain terrain = Terrain.activeTerrains[i];

                    Bounds terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, terrain.terrainData.bounds.size);

                    if (i == 0)
                    {
                        newBounds = terrainBounds;
                    }
                    else
                    {
                        newBounds.Encapsulate(terrainBounds);
                    }
                }

                if(newBounds.size.z > newBounds.size.x)
                {
                    newBounds = new Bounds(newBounds.center, new Vector3(newBounds.size.z, newBounds.size.y, newBounds.size.z)); 
                }
                else if(newBounds.size.x > newBounds.size.z)
                {
                    newBounds = new Bounds(newBounds.center, new Vector3(newBounds.size.x, newBounds.size.y, newBounds.size.x));
                }

                var transform = stamperToolTool.transform;
                transform.position = newBounds.center + new Vector3(1, 0, 1);
                transform.localScale = newBounds.size + new Vector3(1, 0, 1);
            }
            #if GRIFFIN_2020 || GRIFFIN_2021
            else
            {
                Bounds b = GCommon.GetLevelBounds();
                stamperTool.transform.position = new Vector3(b.center.x, b.size.y / 2, b.center.z);
                stamperTool.transform.localScale = new Vector3(b.size.x, b.size.y, b.size.z);
            }
            #endif
        }

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