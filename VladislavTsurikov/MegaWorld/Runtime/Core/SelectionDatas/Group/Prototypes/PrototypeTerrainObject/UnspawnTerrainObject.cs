using System.Collections.Generic;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.API;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject
{
    public static class UnspawnTerrainObject
    {
        public static void Unspawn(List<Prototype> prototypes, bool unspawnSelected)
        {
#if RENDERER_STACK
            foreach (PrototypeTerrainObject proto in prototypes)
            {
                if(unspawnSelected)
                {
                    if(proto.Selected == false)
                    {
                        continue;
                    }
                }

                TerrainObjectRendererAPI.RemoveInstances(proto.ID);
            }
#endif
        }
    }
}