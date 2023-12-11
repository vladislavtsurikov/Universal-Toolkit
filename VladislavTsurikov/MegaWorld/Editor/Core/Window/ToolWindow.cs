#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public abstract class ToolWindow : ComponentStack.Runtime.Component
    {
	    public static int EditorHash = "Editor".GetHashCode();
	    
	    internal void HandleKeyboardEventsInternal()
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                case EventType.Repaint:
                    return;
            }

            HandleKeyboardEvents();
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

        protected override void SetupElement(object[] args = null)
        {
	        OnEnable();
        }

        protected override void OnDisable()
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