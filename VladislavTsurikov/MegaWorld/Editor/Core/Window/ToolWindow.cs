#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;
using Object = UnityEngine.Object;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public abstract class ToolWindow : Runtime_Core_Component
    {
	    private bool _mouseDownHappened;
	    
	    public static int EditorHash = "Editor".GetHashCode();

	    internal void HandleKeyboardEventsInternal()
        {
            switch (Event.current.type)
            {
	            case EventType.MouseDown:
	            {
		            //Second Mouse Button
		            if (Event.current.button == 1)
		            {
			            _mouseDownHappened = true;
		            }
	                
		            break;
	            }
	            case EventType.MouseUp:
	            {
		            _mouseDownHappened = false;
		            break;
	            }
                case EventType.Layout:
                case EventType.Repaint:
                    return;
            }
			
            if (!_mouseDownHappened)
            {
	            HandleKeyboardEvents();
            }
        }

        internal void DoToolInternal()
        {
            if(WindowData.Instance.SelectedData.HasOneSelectedGroup())
            {
	            if(ResourcesControllerEditor.HasSyncError(WindowData.Instance.SelectedData.SelectedGroup))
				{
					return;
				}

	            if(WindowData.Instance.SelectedData.SelectedGroup.PrototypeList.Count == 0)
	            {
		            return;
	            }
            }

            DoTool();
        }

        protected override UniTask SetupComponent(object[] setupData = null)
        {
	        OnEnable();
	        return UniTask.CompletedTask;
        }

        protected override void OnDisableElement()
        {
	        Selected = false;
        }

        protected override void OnSelect()
        {
	        Selection.objects = Array.Empty<Object>();
	        Tools.current = Tool.None;
        }

        protected virtual void OnEnable(){}
        protected virtual void DoTool() {}
        protected virtual void HandleKeyboardEvents(){}

        public virtual bool DisableToolIfUnityToolActive()
        {
	        return true;
        }
    }
}
#endif