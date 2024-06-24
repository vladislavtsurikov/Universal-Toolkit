#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.UnityUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;
using DrawHandles = VladislavTsurikov.MegaWorld.Runtime.Common.Utility.Repaint.DrawHandles;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.Visualisation
{
    public static class PrecisePlaceVisualisation 
    {
        public static void DrawVisualisation(MouseMove mouseMove)
        {
            PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));
            
            if (!settings.MouseActionStack.IsAnyMouseActionActive)
            {
                Vector3 upwards;

                if(settings.Align)
                {
                    upwards = Vector3.Lerp(Vector3.up, mouseMove.Raycast.Normal, settings.WeightToNormal);
                }
                else
                {
                    upwards = Vector3.up;
                }

                TransformAxes.GetRightForward(upwards, out var right, out var forward);

                DrawHandles.DrawXYZCross(mouseMove.Raycast, upwards, right, forward);

                if(ActiveObjectController.PlacedObjectData != null)
                {
                    if(settings.OverlapCheck && settings.VisualizeOverlapCheckSettings)
                    { 
                        Bounds bounds = new Bounds
                        {
                            size = new Vector3(40, 40, 40),
                            center = mouseMove.Raycast.Point
                        };

                        DrawInitialHandle(mouseMove.Raycast.Point, GetCurrentColorFromFitness(mouseMove.Raycast.Point, mouseMove.Raycast));
                    }
                    else if(settings.OverlapCheck)
                    {
                        DrawInitialHandle(mouseMove.Raycast.Point, GetCurrentColorFromFitness(mouseMove.Raycast.Point, mouseMove.Raycast));
                    }
                }
                else
                {
                    DrawInitialHandle(mouseMove.Raycast.Point, GetCurrentColorFromFitness(mouseMove.Raycast.Point, mouseMove.Raycast));
                }
            }
            else
            {
                settings.MouseActionStack.OnRepaint();
            }
        }

        private static void DrawInitialHandle(Vector3 position, Color color)
        {
            UnityUtility.Editor.DrawHandles.HandleButton(0, position, color, color, 0.4f);
        }

        private static Color GetCurrentColorFromFitness(Vector3 position, RayHit rayHit)
        {
            if(ActiveObjectController.PlacedObjectData == null)
            {
                return Color.red;
            }

            Color color = Color.green;
            
            PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));

            if(settings.OverlapCheck)
            {
                Vector3 scale = ActiveObjectController.PlacedObjectData.GameObject.transform.localScale;
                Quaternion rotation = ActiveObjectController.PlacedObjectData.GameObject.transform.rotation;

                Instance instance = new Instance(position, scale, rotation);

                PlacedObjectPrototype proto = ActiveObjectController.PlacedObjectData.Proto;

                OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

                if(!OverlapCheckSettings.RunOverlapCheck(ActiveObjectController.PlacedObjectData.Proto.GetType(), overlapCheckSettings, proto.Extents, instance))
                {
                    return Color.red;
                }
            }

            return color;
        }
    }
}
#endif