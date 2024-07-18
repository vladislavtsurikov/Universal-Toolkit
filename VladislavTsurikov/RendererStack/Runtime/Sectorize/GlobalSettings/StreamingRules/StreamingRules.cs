using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules
{
    [Name("Streaming Rules")]
    public class StreamingRules : GlobalComponent
    {
        public StreamingRuleComponentStack StreamingRuleComponentStack = new StreamingRuleComponentStack();

        protected override void SetupComponent(object[] setupData = null)
        {
            StreamingRuleComponentStack ??= new StreamingRuleComponentStack();
            StreamingRuleComponentStack.Setup();
        }
    }
}