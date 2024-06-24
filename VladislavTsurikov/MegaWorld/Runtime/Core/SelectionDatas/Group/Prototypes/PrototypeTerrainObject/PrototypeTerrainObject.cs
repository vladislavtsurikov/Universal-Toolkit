using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.UnityUtility.Runtime;
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

        public int RendererPrototypeID
        {
            get
            {
                RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject rendererPrototype = 
                    (RendererStack.Runtime.TerrainObjectRenderer.PrototypeTerrainObject)PrototypesStorage.Instance.GetPrototype(Prefab, typeof(TerrainObjectRenderer));

                if (rendererPrototype == null)
                {
                    return -1;
                }
                
                return rendererPrototype.ID; 
            }
        }


        public override void SetupPrototype()
        {
            AllAvailableTerrainObjectPrototypes.AddPrototype(this);
        }

        public override void OnDisablePrototype()
        {
            AllAvailableTerrainObjectPrototypes.RemovePrototype(this);
        }
#endif
        
        public override bool IsSamePrototypeObject(Object obj)
        {
            GameObject go = (GameObject)obj;

            return GameObjectUtility.IsSameGameObject(go, Prefab);
        }
    }
}