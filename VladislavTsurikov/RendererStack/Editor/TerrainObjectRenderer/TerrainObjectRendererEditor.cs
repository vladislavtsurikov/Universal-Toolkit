#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Editor.Core.RendererSystem;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer
{
    [ElementEditor((typeof(Runtime.TerrainObjectRenderer.TerrainObjectRenderer)))]
    public class TerrainObjectRendererEditor : PrototypeRendererEditor
    {
        private readonly TerrainObjectRendererMenu _menu = new TerrainObjectRendererMenu();
        
        public override RendererMenu GetRendererMenu()
        {
            return _menu;
        }
    }
}
#endif
