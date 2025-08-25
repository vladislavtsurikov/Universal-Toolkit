using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules
{
    [Name("Streaming Rules")]
    public class StreamingRules : GlobalComponent
    {
        public StreamingRuleComponentStack StreamingRuleComponentStack = new();

        protected override UniTask SetupComponent(object[] setupData = null)
        {
            StreamingRuleComponentStack ??= new StreamingRuleComponentStack();
            _ = StreamingRuleComponentStack.Setup();

            return UniTask.CompletedTask;
        }
    }
}
