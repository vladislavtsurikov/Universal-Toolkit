using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Core.Runtime.IconStack.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.SelectionDatas;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.API;
using VladislavTsurikov.Utility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject
{
    [MenuItem("Terrain Object")]
    [DropObjects(new[]{typeof(GameObject)})]
    [MissingIconsWarning("Drag & Drop Prefabs Here")]
    [AddDefaultGroupComponents(new []{typeof(ContainerForGameObjects)})]
    public class PrototypeTerrainObject : PlacedObjectPrototype
    {
#if RENDERER_STACK
        public RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject RendererPrototype => 
            (RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject)TerrainObjectRendererAPI.AddMissingPrototype(Prefab);

        public override int ID
        {
            get
            {
                RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject rendererPrototype = 
                    (RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject)PrototypesStorage.Instance.GetPrototype(Prefab, typeof(TerrainObjectRenderer));

                if (rendererPrototype == null)
                {
                    return _id;
                }
                
                return rendererPrototype.ID; 
            }
        }
#endif
        
        public override bool IsSamePrototypeObject(Object obj)
        {
            GameObject go = (GameObject)obj;

            return GameObjectUtility.IsSameGameObject(go, Prefab);
        }
    }
}