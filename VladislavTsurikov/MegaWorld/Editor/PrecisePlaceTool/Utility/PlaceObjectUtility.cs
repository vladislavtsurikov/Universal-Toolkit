#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.MegaWorld.Editor.Common.Window;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.Utility
{
    public static class PlaceObjectUtility 
    {
        public static PlacedObjectData DragPlace(Group group, PlacedObjectPrototype proto, RayHit rayHit, MouseMove mouseMove)
        {
            PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));

            PlacedObjectData placedObjectData = TryToPlace(group, proto, rayHit);

            if(placedObjectData == null)
            {
                return null;
            }

            if(settings.AlongStroke)
            {
                ObjectActions.RotateAlongStroke(placedObjectData.GameObject, mouseMove);
            }

            return placedObjectData;
        }

        public static PlacedObjectData TryToPlace(Group group, PlacedObjectPrototype proto, RayHit rayHit)
        {
            if(proto == null)
            {
                return null;
            }

            PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));

            InstanceData instanceData = GetInstancedDataForSpawn(proto, rayHit, settings.RememberPastTransform);

            if(!CanPlace(proto, instanceData))
            {
                return null;
            }

            return Place(group, proto, rayHit, instanceData);
        }

        private static PlacedObjectData Place(Group group, PlacedObjectPrototype proto, RayHit rayHit, InstanceData instanceData)
        {
            PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));

            GameObject gameObject = GameObjectUtility.Instantiate(proto.Prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);

            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);
            }
            
            if(settings.Align)
            {
                ObjectActions.AlignObject(gameObject, rayHit.Normal, settings.WeightToNormal);
            }

            PrecisePlaceSettings precisePlaceSettings = (PrecisePlaceSettings)proto.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceSettings));

            ObjectActions.ChangePositionOffset(gameObject, precisePlaceSettings.PositionOffset);

            return new PlacedObjectData(proto, gameObject);
        }

        public static bool CanPlace(PlacedObjectPrototype proto, GameObject go)
        {
            InstanceData instanceData = new InstanceData(go);
            return CanPlace(proto, instanceData);
        }

        private static bool CanPlace(PlacedObjectPrototype proto, InstanceData instanceData)
        {
            PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));
            
            if(settings.OverlapCheck)
            {
                OverlapCheckSettings overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

                if(proto is PrototypeGameObject)
                {
                    if(!OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, instanceData))
                    {
                        return false;
                    }
                }
                else
                {
                    if(!OverlapCheckSettings.RunOverlapCheck(proto.GetType(), overlapCheckSettings, proto.Extents, instanceData))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        private static InstanceData GetInstancedDataForSpawn(PlacedObjectPrototype proto, RayHit rayHit, bool rememberPastTransform)
        {
            PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));

            Vector3 scaleFactor = Vector3.one;
            Quaternion rotation = Quaternion.identity;

            if(rememberPastTransform && proto.PastTransform != null)
            {
                scaleFactor = proto.PastTransform.Scale;
                rotation = proto.PastTransform.Rotation;
            }

            InstanceData instanceData = new InstanceData(rayHit.Point, scaleFactor, rotation);

            if(settings.UseTransformComponents)
            {
                TransformComponentSettings transformComponentSettings = (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
                transformComponentSettings.Stack.SetInstanceData(ref instanceData, 1, rayHit.Normal);
            }

            return instanceData;
        }
    }
}
#endif