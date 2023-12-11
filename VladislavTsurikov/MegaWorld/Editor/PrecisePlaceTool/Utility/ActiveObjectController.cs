#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.API;
using VladislavTsurikov.Runtime;
using VladislavTsurikov.Undo.Editor.UndoActions;
#if RENDERER_STACK
#endif

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.Utility
{
    public static class ActiveObjectController
    {
        private static PlacedObjectData _placedObjectData;
        public static PlacedObjectData PlacedObjectData
        {
            get
            {
                return _placedObjectData;
            }
            set
            {
                if(value == null)
                {
                    return;
                }

                if(_placedObjectData != null)
                {
                    PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));

                    settings.MouseActionStack.End();
                    
                    if(_placedObjectData.Proto.GetType() == typeof(PrototypeGameObject))
                    {
                        GameObjectCollider.Runtime.GameObjectCollider.RegisterGameObjectToCurrentScene(_placedObjectData.GameObject);
                        Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(_placedObjectData.GameObject));
                        _placedObjectData = value;
                    }
                    else if(_placedObjectData.Proto.GetType() == typeof(PrototypeTerrainObject))
                    {
#if RENDERER_STACK
                        InstanceData instanceData = new InstanceData(_placedObjectData.GameObject);
                        
                        GameObject prefab = PrefabUtility.GetOutermostPrefabInstanceRoot(_placedObjectData.GameObject);

                        TerrainObjectRendererAPI.AddInstance(prefab, instanceData.Position, instanceData.Scale, instanceData.Rotation);
                        Object.DestroyImmediate(_placedObjectData.GameObject);
                        _placedObjectData = value;
#endif
                    }
                }
                else
                {
                    _placedObjectData = value;
                }
            }
        }

        public static void DestroyObjectIfNecessary()
        {
            if(_placedObjectData == null)
            {
                return;
            }

            if(_placedObjectData.GameObject == null || _placedObjectData.Proto.Selected == false)
            {
                DestroyObject();
            }
        }

        public static void DestroyObject()
        {            
            if(_placedObjectData != null && _placedObjectData.GameObject != null)
            {
                Object.DestroyImmediate(_placedObjectData.GameObject);
            }
            
            _placedObjectData = null;
        }
    }
}
#endif