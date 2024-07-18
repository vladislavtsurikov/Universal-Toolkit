using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    public partial class Sectorize
    {
        private CameraManager _cameraManager;
        private StreamingRules _streamingRules;
        private AsynchronousLoading _asynchronousLoading;
        private ImmediatelyLoading _immediatelyLoading;
        private PreventingUnloading _preventingUnloading;
        private Caching _caching;

        public CameraManager CameraManager
        {
            get
            {
                return _cameraManager ??= (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));
            }
        }

        public StreamingRules StreamingRules
        {
            get
            {
                return _streamingRules ??= (StreamingRules)Core.GlobalSettings.GlobalSettings.Instance.GetElement(typeof(StreamingRules), GetType());
            }
        }
        
        public ImmediatelyLoading ImmediatelyLoading
        {
            get
            {
                return _immediatelyLoading ??= StreamingRules.StreamingRuleComponentStack.GetElement<ImmediatelyLoading>();
            }
        }

        public AsynchronousLoading AsynchronousLoading
        {
            get
            {
                return _asynchronousLoading ??= StreamingRules.StreamingRuleComponentStack.GetAndAutoUpdateComponent<AsynchronousLoading>(component => _asynchronousLoading = component);
            }
        }
        
        public PreventingUnloading PreventingUnloading
        {
            get
            {
                return _preventingUnloading ??= StreamingRules.StreamingRuleComponentStack.GetAndAutoUpdateComponent<PreventingUnloading>(component => _preventingUnloading = component);
            }
        }

        public Caching Caching
        {
            get
            {
                return _caching ??= StreamingRules.StreamingRuleComponentStack.GetAndAutoUpdateComponent<Caching>(component => _caching = component);
            }
        }
    }
}