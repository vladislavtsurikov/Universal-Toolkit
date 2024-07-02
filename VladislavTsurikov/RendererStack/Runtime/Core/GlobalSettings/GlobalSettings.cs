using System;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;
#if UNITY_EDITOR 
using VladislavTsurikov.RendererStack.Editor.Core.GlobalSettings;
#endif

namespace VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings
{
    [LocationAsset("RendererStack/GlobalSettings")]
    public class GlobalSettings : SerializedScriptableObjectSingleton<GlobalSettings>
    {
        [OdinSerialize]
        public RenderersGlobalComponentStack RenderersGlobalComponentStack = new RenderersGlobalComponentStack();
        
#if UNITY_EDITOR
        private RenderersGlobalComponentStackEditor _renderersGlobalComponentStackEditor;
        
        public RenderersGlobalComponentStackEditor RenderersGlobalComponentStackEditor
        {
            get
            {
                if (_renderersGlobalComponentStackEditor == null)
                {
                    _renderersGlobalComponentStackEditor = new RenderersGlobalComponentStackEditor(RenderersGlobalComponentStack);
                }

                return _renderersGlobalComponentStackEditor;
            }
        }
#endif

        public void OnDisable()
        {
            RenderersGlobalComponentStack.OnDisable();
        }

        public void Setup()
        {
            RenderersGlobalComponentStack.Setup();
        }
        
        public GlobalComponent GetElement(Type type, Type rendererType)
        {
            return RenderersGlobalComponentStack.GetElement(rendererType, type);
        }

#if UNITY_EDITOR
        public void DrawGizmos(Renderer renderer)
        {
            if (renderer == null)
            {
                return;
            }
            
            RendererGlobalComponentStack rendererGlobalComponentStack = RenderersGlobalComponentStack.GetRendererGlobalComponentStack(renderer.GetType());

            if (rendererGlobalComponentStack == null)
            {
                return;
            }
            
            foreach (var settings in rendererGlobalComponentStack.ComponentStack.ElementList)
            {
                if(settings.Selected)
                {
                    settings.OnSelectedDrawGizmos();
                }

                settings.OnDrawGizmos();
            }
        }
#endif
    }
}