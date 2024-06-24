using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem
{
    [MenuItem("Extension")]
    public class ExtensionSystem : GlobalComponent
    {
        [OdinSerialize]
        public ComponentStackOnlyDifferentTypes<Extension> ExtensionStack = new ComponentStackOnlyDifferentTypes<Extension>();

        protected override void SetupElement(object[] args = null)
        {
            ExtensionStack.Setup();
        }
    }
}