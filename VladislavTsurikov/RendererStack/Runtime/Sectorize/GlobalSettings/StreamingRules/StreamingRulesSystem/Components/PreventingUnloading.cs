using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core.Extensions;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem
{
    [Name("Preventing Unloading")]
    public class PreventingUnloading : StreamingRule
    {
        private ImmediatelyLoading _immediatelyLoading;
        private AsynchronousLoading _asynchronousLoading;
        
        private ImmediatelyLoading ImmediatelyLoading
        {
            get
            {
                return _immediatelyLoading ??= StreamingRuleComponentStack.GetElement<ImmediatelyLoading>();
            }
        }
        
        public AsynchronousLoading AsynchronousLoading
        {
            get
            {
                return _asynchronousLoading ??= StreamingRuleComponentStack.GetAndAutoUpdateComponent<AsynchronousLoading>(component => _asynchronousLoading = component);
            }
        }
        
        [OdinSerialize]
        private float _maxDistance = 4400;

        public float MaxDistance
        {
            get
            {
                float maxLoadingDistance = GetMaxLoadingDistance();
                if (_maxDistance < maxLoadingDistance)
                {
                    _maxDistance = maxLoadingDistance; 
                }
                
                return Mathf.Max(maxLoadingDistance, _maxDistance);
            }
            set => _maxDistance = Mathf.Max(GetMaxLoadingDistance(), value);
        }
        
        private float GetMaxLoadingDistance()
        {
            return Mathf.Max(ImmediatelyLoading.MaxDistance, AsynchronousLoading.IsValid() ? AsynchronousLoading.MaxDistance : 0);
        }
    }
}