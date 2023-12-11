using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.MegaWorld.Runtime.TextureStamperTool
{
    public class TextureStamperArea : Common.Stamper.Area
    {
        public bool UseSpawnCells;
        public float CellSize = 1000;
        public List<Bounds> CellList = new List<Bounds>();

        public bool UseMask;
        public MaskType MaskType = MaskType.Procedural;
        public ProceduralMask ProceduralMask = new ProceduralMask();
        public CustomMasks CustomMasks = new CustomMasks();
        
        public bool ShowCells = true;

        public void CreateCells()
        {
            CellList.Clear();

            Bounds expandedBounds = new Bounds(Bounds.center, Bounds.size);
            expandedBounds.Expand(new Vector3(CellSize * 2f, 0, CellSize * 2f));

            int cellXCount = Mathf.CeilToInt(Bounds.size.x / CellSize);
            int cellZCount = Mathf.CeilToInt(Bounds.size.z / CellSize);

            Vector2 corner = new Vector2(Bounds.center.x - Bounds.size.x / 2f, Bounds.center.z - Bounds.size.z / 2f);

            for (int x = 0; x <= cellXCount - 1; x++)
            {
                for (int z = 0; z <= cellZCount - 1; z++)
                {
                    Rect rect = new Rect(
                        new Vector2(CellSize * x + corner.x, CellSize * z + corner.y),
                        new Vector2(CellSize, CellSize));

                    var bounds = RectExtension.CreateBoundsFromRect(rect, Bounds.center.y, Bounds.size.y);

                    CellList.Add(bounds);
                }
            }
        }
        
        public override void OnSetAreaBounds()
        {
            if(UseSpawnCells)
            {
                CellList.Clear();
            }
        }
        
        public override Texture2D GetCurrentRaw()
        {
            if(UseMask == false || UseSpawnCells)
            {
                return Texture2D.whiteTexture;
            }

            switch (MaskType)
            {
                case MaskType.Custom:
                {
                    Texture2D texture = CustomMasks.GetSelectedBrush();

                    return texture;
                }
                case MaskType.Procedural:
                {
                    Texture2D texture = ProceduralMask.Mask;

                    return texture;
                }
            }

            return Texture2D.whiteTexture;
        }

        public BoxArea GetAreaVariablesFromSpawnCell(RayHit hit, Bounds bounds)
        {
            BoxArea area = new BoxArea(hit, bounds.size.x);

            return area;
        }
    }
}