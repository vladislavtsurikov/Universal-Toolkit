using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    [Name("Asynchronous Loading")]
    public class AsynchronousLoading : StreamingRule
    {
        private ImmediatelyLoading _immediatelyLoading;
        
        private ImmediatelyLoading ImmediatelyLoading
        {
            get
            {
                return _immediatelyLoading ??= StreamingRuleComponentStack.GetElement<ImmediatelyLoading>();
            }
        }
        
        [OdinSerialize]
        private float _maxDistance = 4000;

        public float MaxDistance
        {
            get
            {
                if (_maxDistance < ImmediatelyLoading.MaxDistance)
                {
                    _maxDistance = ImmediatelyLoading.MaxDistance; 
                }
                
                return Mathf.Max(ImmediatelyLoading.MaxDistance, _maxDistance);
            }
            set => _maxDistance = Mathf.Max(ImmediatelyLoading.MaxDistance, value);
        }


        public float MaxLoadingScenePause = 1;
    }
}