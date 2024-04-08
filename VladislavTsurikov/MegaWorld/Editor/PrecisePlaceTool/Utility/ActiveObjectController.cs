#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.API;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.RendererData;
using VladislavTsurikov.Undo.Editor.Actions.GameObject;
using VladislavTsurikov.Undo.Editor.Actions.TerrainObjectRenderer;
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
                if (_placedObjectData == null || _placedObjectData.GameObject == null)
                {
                    return null;
                }
                
                return _placedObjectData;
            }
            set
            {
                if(value == null || value.GameObject == null)
                {
                    return;
                }

                if(_placedObjectData != null && _placedObjectData.GameObject != null)
                {
                    PrecisePlaceToolSettings settings = (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool), typeof(PrecisePlaceToolSettings));

                    settings.MouseActionStack.End();
                    
                    if(_placedObjectData.Proto.GetType() == typeof(PrototypeGameObject))
                    {
                        GameObjectCollider.Runtime.GameObjectCollider.RegisterGameObjectToCurrentScene?.Invoke(_placedObjectData.GameObject);
                        Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedGameObject(_placedObjectData.GameObject));
                        _placedObjectData = value;
                    }
#if RENDERER_STACK
                    else if(_placedObjectData.Proto.GetType() == typeof(PrototypeTerrainObject))
                    {
                        PrototypeTerrainObject prototypeTerrainObject = (PrototypeTerrainObject)_placedObjectData.Proto;

                        Transform transform = _placedObjectData.GameObject.transform;
                        
                        TerrainObjectInstance terrainObjectInstance = TerrainObjectRendererAPI.AddInstance(prototypeTerrainObject.RendererPrototype, transform.position, transform.lossyScale, transform.rotation);
                        Undo.Editor.Undo.RegisterUndoAfterMouseUp(new CreatedTerrainObject(terrainObjectInstance));
                        Object.DestroyImmediate(_placedObjectData.GameObject);
                        _placedObjectData = value;
                    }
#endif
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

            if(_placedObjectData.Proto.Selected == false)
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