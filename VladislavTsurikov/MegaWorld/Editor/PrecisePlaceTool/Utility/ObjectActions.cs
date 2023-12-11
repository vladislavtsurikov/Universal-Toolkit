#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.Utility
{
    public static class ObjectActions 
    {
        public static void UpdateTransform(RayHit rayHit)
        {
            PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));
            
            if(ActiveObjectController.PlacedObjectData != null)
            {
                if(!settings.MouseActionStack.IsAnyMouseActionActive)
                {
                    if(settings.EnableSnapMove)
                    {
                        ActiveObjectController.PlacedObjectData.GameObject.transform.position = Snapping.Snap(rayHit.Point, settings.SnapMove);
                    }
                    else
                    {
                        ActiveObjectController.PlacedObjectData.GameObject.transform.position = rayHit.Point;
                    }

                    PrecisePlaceSettings precisePlaceSettings = (PrecisePlaceSettings)ActiveObjectController.PlacedObjectData.Proto.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceSettings));

                    ChangePositionOffset(ActiveObjectController.PlacedObjectData.GameObject, precisePlaceSettings.PositionOffset);

                    if(settings.Align)
                    {
                        AlignObject(ActiveObjectController.PlacedObjectData.GameObject, rayHit.Normal, settings.WeightToNormal);
                    }
                }
            }
        }

        public static void ChangePositionOffset(GameObject gameObject, float offset)
        {
            gameObject.transform.position += new Vector3(0, offset, 0);
        }

        public static void AlignObject(GameObject gameObject, Vector3 normal, float weightToNormal)
        {
            Transform objectTransform = gameObject.transform;

            Quaternion normalRotation = Quaternion.FromToRotation(Vector3.up, normal);
            objectTransform.rotation = Quaternion.Lerp(Quaternion.identity, normalRotation, weightToNormal);
        }

        public static void RotateAlongStroke(GameObject gameObject, MouseMove mouseMove)
        {
            PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));

            Vector3Ex.GetOrientation(mouseMove.Raycast.Normal, settings.Align ? FromDirection.SurfaceNormal : FromDirection.Y, settings.WeightToNormal,
                out var upwards, out _, out var forward);

            var strokeForward = Vector3.Cross(mouseMove.StrokeDirection, upwards);

            if (strokeForward.magnitude > 0.001f)
                forward = strokeForward;

            gameObject.transform.rotation = Quaternion.LookRotation(forward, upwards);
        }
    }
}
#endif